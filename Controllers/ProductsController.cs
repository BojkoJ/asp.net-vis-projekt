using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using Projekt.Models.ViewModels;
using Projekt.Services;

namespace Projekt.Controllers
{
    public class ProductsController : Controller
    {
        // ---------------------------------------------- Table Data Gateway BEGIN ----------------------------------------------
        private readonly ProductManager _db;

        public ProductsController(ProductManager db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult ProductsByCategory(
            [FromQuery] int categoryId,
            [FromQuery] List<string> selectedSizes = null,
            [FromQuery] List<string> selectedColors = null
        )
        {
            // Načteme produkty pro zvolenou kategorii
            var products = _db.GetProductsByCategory(categoryId, limit: 20);

            // Definujeme pevné velikosti a barvy pro každou kategorii
            List<string> availableSizes = new List<string>();
            List<string> availableColors = new List<string>();

            switch (categoryId)
            {
                case 1: // Rukavice
                    availableSizes = new List<string> { "8oz", "10oz", "12oz", "14oz", "16oz", "18oz" };
                    availableColors = new List<string>
                    {
                        "Černá",
                        "Bílá",
                        "Modrá",
                        "Červená",
                        "Vícebarevné",
                    };
                    break;
                case 2: // Bandáže
                    availableSizes = new List<string> { "2.5m", "3m", "4m" };
                    availableColors = new List<string>
                    {
                        "Černá",
                        "Bílá",
                        "Modrá",
                        "Červená",
                        "Zelená",
                        "Vícebarevné",
                    };
                    break;
                case 3: // Chrániče
                    availableSizes = new List<string> { "S", "M", "L" };
                    availableColors = new List<string>
                    {
                        "Černá",
                        "Bílá",
                        "Modrá",
                        "Červená",
                        "Tyrkysová",
                        "Vícebarevné",
                    };
                    break;
            }

            // Mapování vybraných barev na diakritické verze
            if (selectedColors != null && selectedColors.Count > 0)
            {
                selectedColors = selectedColors.Select(color => MapColor(color)).ToList();
            }

            // Filtrování podle velikostí
            if (selectedSizes != null && selectedSizes.Count > 0)
            {
                products = products
                    .Where(p => p.Variants.Any(v => selectedSizes.Contains(v.Size)))
                    .ToList();
            }

            // Filtrování podle barev
            if (selectedColors != null && selectedColors.Count > 0)
            {
                products = products
                    .Where(p => p.Variants.Any(v => selectedColors.Contains(v.Color)))
                    .ToList();
            }

            // Pokud nebyly nalezeny žádné produkty, zobrazíme toast notifikaci
            if (products.Count == 0)
            {
                products = _db.GetProductsByCategory(categoryId, limit: 20);
                TempData["NoResults"] =
                    "Nebyly nalezeny žádné produkty odpovídající vašim kritériím.";
            }

            var categoryName = products.Count > 0 ? products[0].Category.Name : "Neznámá kategorie";

            // Vytvoříme ViewModel pro zobrazení produktů a filtrů
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = categoryId,
                CategoryName = categoryName,
                Products = products,
                AvailableSizes = availableSizes,
                SelectedSizes = selectedSizes,
                AvailableColors = availableColors,
                SelectedColors = selectedColors,
            };

            return View(viewModel);
        }

        private string MapColor(string color)
        {
            return color switch
            {
                "Cerna" => "Černá",
                "Bila" => "Bílá",
                "Modra" => "Modrá",
                "Cervena" => "Červená",
                "Vicebarevne" => "Vícebarevné",
                "Zelena" => "Zelená",
                "Tyrkysova" => "Tyrkysová",
                _ => color,
            };
        }

        // ---------------------------------------------- Table Data Gateway END ----------------------------------------------
        // ---------------------------------------------- Row Data Gateway BEGIN ----------------------------------------------
        private readonly string _connectionString;

        /*public ProductsController(string connectionString)
        {
            _connectionString = connectionString;
        }*/

        [HttpGet]
        public IActionResult ProductDetails(int productId)
        {
            var productGateway = new ProductRowGateway(_connectionString);
            productGateway.Load(productId);

            var productViewModel = new ProductViewModel
            {
                ProductId = productGateway.ProductId,
                Name = productGateway.Name,
                Description = productGateway.Description,
                Price = productGateway.Price,
                ImgUrl = productGateway.ImgUrl,
            };

            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult SaveProduct(ProductViewModel model)
        {
            var productGateway = new ProductRowGateway(_connectionString)
            {
                ProductId = model.ProductId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImgUrl = model.ImgUrl,
            };

            productGateway.Save();
            return RedirectToAction("ProductDetails", new { productId = productGateway.ProductId });
        }
        // ---------------------------------------------- Row Data Gateway END ----------------------------------------------
    }
}
