using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Projekt.Controllers;
using Projekt.Models;
using Projekt.Models.ViewModels;
using Projekt.Services;

namespace Projekt.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderManager _orderManager;
        private readonly IConfiguration _configuration;

        // Konstruktor s injektovaným UserManagerem
        public OrderController(OrderManager orderManager, IConfiguration configuration)
        {
            _orderManager = orderManager;
            _configuration = configuration;
        }

        // Zobrazení obsahu košíku
        [HttpGet]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cart(string cartData)
        {
            if (string.IsNullOrEmpty(cartData))
            {
                TempData["ErrorMessage"] = "Košík je prázdný.";
                return RedirectToAction("Cart");
            }

            // Deserializace JSON do dynamického objektu
            var cartItemsDynamic = JsonConvert.DeserializeObject<List<dynamic>>(cartData);

            // Mapování na OrderItemViewModel
            var cartItems = cartItemsDynamic
                .Select(item => new OrderItemViewModel
                {
                    ProductVariantId = int.TryParse((string)item.productId, out var id) ? id : 0,
                    Quantity = item.quantity,
                    Price = item.price,
                    ProductName = item.name,
                })
                .ToList();

            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "Košík je prázdný.";
                return RedirectToAction("Cart");
            }

            var itemsJson = JsonConvert.SerializeObject(cartItems);

            HttpContext.Session.SetString("CartItems", itemsJson);

            var totalPrice = cartItems.Sum(item => item.Price * item.Quantity);
            HttpContext.Session.SetString("TotalPrice", totalPrice.ToString());

            return RedirectToAction("SelectShipping");
        }

        [HttpGet]
        public IActionResult SelectShipping()
        {
            return View(new ShippingViewModel());
        }

        [HttpPost]
        public IActionResult SelectShipping(ShippingViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uložte vybraný způsob dopravy do session nebo databáze
                HttpContext.Session.SetString("ShippingMethod", model.ShippingMethod);

                // Přesměrujte na další krok
                if (model.ShippingMethod == "Zásilkovna")
                {
                    return RedirectToAction("SelectPickupPoint");
                }
                else
                {
                    return RedirectToAction("EnterDeliveryAddress");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SelectPickupPoint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectPickupPoint(PickupPointViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uložte vybraný bod do session nebo databáze
                HttpContext.Session.SetString("PickupPoint", model.SelectedPoint);

                // Reset doručovací adresy
                HttpContext.Session.Remove("DeliveryAddress");

                // Přesměrujte na způsob platby
                return RedirectToAction("SelectPaymentMethod");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EnterDeliveryAddress()
        {
            return View(new DeliveryAddressViewModel());
        }

        [HttpPost]
        public IActionResult EnterDeliveryAddress(DeliveryAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uložte adresu do session nebo databáze
                HttpContext.Session.SetString("DeliveryAddress", model.FullAddress);

                // Reset zvoleného výdejního místa
                HttpContext.Session.Remove("PickupPoint");

                // Přesměrujte na způsob platby
                return RedirectToAction("SelectPaymentMethod");
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SelectPaymentMethod()
        {
            return View(new PaymentMethodViewModel());
        }

        [HttpPost]
        public IActionResult SelectPaymentMethod(PaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uložte způsob platby do session nebo databáze
                HttpContext.Session.SetString("PaymentMethod", model.SelectedPaymentMethod);

                // Přesměrujte na potvrzení objednávky
                return RedirectToAction("ConfirmOrder");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ConfirmOrder()
        {
            // Načtení dat ze session
            var itemsJson = HttpContext.Session.GetString("CartItems");
            var totalPrice = HttpContext.Session.GetString("TotalPrice");
            var shippingMethod = HttpContext.Session.GetString("ShippingMethod");
            var pickupPoint = HttpContext.Session.GetString("PickupPoint");
            var deliveryAddress = HttpContext.Session.GetString("DeliveryAddress");
            var paymentMethod = HttpContext.Session.GetString("PaymentMethod");

            var orderItems = string.IsNullOrEmpty(itemsJson)
                ? new List<OrderItemViewModel>()
                : JsonConvert.DeserializeObject<List<OrderItemViewModel>>(itemsJson);

            // Sestavení modelu pro rekapitulaci
            var model = new OrderSummaryViewModel
            {
                Items = orderItems,
                TotalPrice = totalPrice != null ? decimal.Parse(totalPrice) : 0,

                ShippingMethod = shippingMethod,
                PickupPoint = pickupPoint,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,

                FirstName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ConfirmOrder(string firstName, string lastName, string email)
        {
            System.Console.WriteLine("ConfirmOrder");
            // Načtení dat ze session
            var itemsJson = HttpContext.Session.GetString("CartItems");
            var shippingMethod = HttpContext.Session.GetString("ShippingMethod");
            var pickupPoint = HttpContext.Session.GetString("PickupPoint");
            var deliveryAddress = HttpContext.Session.GetString("DeliveryAddress");
            var paymentMethod = HttpContext.Session.GetString("PaymentMethod");

            if (
                string.IsNullOrEmpty(itemsJson)
                || string.IsNullOrEmpty(shippingMethod)
                || string.IsNullOrEmpty(paymentMethod)
            )
            {
                // Pokud chybí některá data, vraťte chybu nebo přesměrujte uživatele zpět
                TempData["ErrorMessage"] =
                    "Došlo k problému při zpracování objednávky. Zkuste to prosím znovu.";
                return RedirectToAction("Cart");
            }

            // Převod položek košíku ze session
            var items = JsonConvert.DeserializeObject<List<OrderItemViewModel>>(itemsJson);

            // Vytvoření kompletního modelu objednávky
            var model = new OrderSummaryViewModel
            {
                Items = items,
                TotalPrice = items.Sum(i => i.Price * i.Quantity),
                ShippingMethod = shippingMethod,
                PickupPoint = pickupPoint,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
            };

            // Zpracování objednávky
            if (ModelState.IsValid)
            {
                // vyčištění session
                HttpContext.Session.Remove("CartItems");
                HttpContext.Session.Remove("TotalPrice");
                HttpContext.Session.Remove("ShippingMethod");
                HttpContext.Session.Remove("PickupPoint");
                HttpContext.Session.Remove("DeliveryAddress");
                HttpContext.Session.Remove("PaymentMethod");

                // Uložení objednávky
                _orderManager.CreateOrder(model);

                return RedirectToAction("OrderConfirmation");
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            return RedirectToAction("OrderConfirmation");
        }

        // Stránka s potvrzením objednávky
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
