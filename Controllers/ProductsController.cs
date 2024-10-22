using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using Projekt.Services;

namespace Projekt.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DatabaseManager _db;

        public ProductsController(DatabaseManager db)
        {
            _db = db;
        }

        public IActionResult ProductsByCategory(int categoryId, List<string> selectedSizes = null)
        {
            // Načteme produkty pro zvolenou kategorii
            var products = _db.GetProductsByCategory(categoryId, limit: 20); // Pouze prvních 20 produktů

            // Definujeme pevné velikosti pro každou kategorii
            List<string> availableSizes = new List<string>();

            switch (categoryId)
            {
                case 1: // Rukavice
                    availableSizes = new List<string> { "8oz", "10oz", "12oz", "14oz", "16oz", "18oz" };
                    break;
                case 2: // Bandáže
                    availableSizes = new List<string> { "2.5m", "3m", "4m" };
                    break;
                case 3: // Chrániče
                    availableSizes = new List<string> { "S", "M", "L" };
                    break;
            }

            // Filtrování podle velikostí
            if (selectedSizes != null && selectedSizes.Count > 0)
            {
                var filteredProducts = products
                    .Where(p => p.Variants.Any(v => selectedSizes.Contains(v.Size)))
                    .ToList();

                // Pokud jsou nalezeny produkty, zobrazíme je
                if (filteredProducts.Count > 0)
                {
                    products = filteredProducts;
                }
                else
                {
                    // Pokud nejsou nalezeny produkty, znovu načteme prvních 20 produktů a zobrazíme toast notifikaci
                    products = _db.GetProductsByCategory(categoryId, limit: 20);
                    TempData["NoResults"] =
                        "Nebyly nalezeny žádné produkty odpovídající vašim kritériím.";
                }
            }

            var categoryName = products.Count > 0 ? products[0].Category.Name : "Neznámá kategorie";

            // Vytvoříme ViewModel pro zobrazení produktů, dostupných velikostí a vybraných filtrů
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = categoryId,
                CategoryName = categoryName,
                Products = products,
                AvailableSizes = availableSizes,
                SelectedSizes =
                    selectedSizes // Předáme vybrané velikosti do view
                ,
            };

            return View(viewModel);
        }
    }
}
