using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projekt.DataMappers;
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
            [FromQuery] List<string> selectedColors = null,
            [FromQuery] int limit = 6, // Načteme 6 produktů na začátek
            [FromQuery] int offset = 0 // Offset pro Lazy Load
        )
        {
            // Nastavíme maximální počet produktů na stránce
            int maxProducts = 18;

            // Pokud by offset přesáhl maximální počet produktů, nebudeme již načítat více produktů
            if (offset >= maxProducts)
            {
                return NoContent(); // Vrátíme prázdnou odpověď
            }

            // Zajistíme, že nenačteme více než zbývající počet produktů
            int actualLimit = Math.Min(limit, maxProducts - offset);

            // Načteme produkty pro zvolenou kategorii s použitím offsetu a limitu
            var products = _db.GetProductsByCategory(categoryId, actualLimit, offset); // Použití actualLimit

            // Pokud jde o AJAX volání, vrátíme pouze partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", products);
            }
            else { }

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
            if (products.Count == 0 && offset == 0) // Pouze pokud jsme na začátku, jinak načítáme dál
            {
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
        public IActionResult ProductsByCategory_RowGateway(
            [FromQuery] int categoryId,
            [FromQuery] List<string> selectedSizes = null,
            [FromQuery] List<string> selectedColors = null
        )
        {
            var products = ProductRowGateway.GetProductsByCategory(categoryId, _connectionString);

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

            // Vytvoříme ViewModel pro zobrazení produktů a filtrů
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = categoryId,
                CategoryName = products.Count > 0 ? products[0].Name : "Neznámá kategorie",
                Products = products
                    .Select(p => new Product
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImgUrl = p.ImgUrl,
                        Variants = p.Variants,
                    })
                    .ToList(),
                AvailableSizes = selectedSizes,
                SelectedSizes = selectedSizes,
                AvailableColors = selectedColors,
                SelectedColors = selectedColors,
            };

            return View(viewModel);
        }

        // ---------------------------------------------- Row Data Gateway END ----------------------------------------------
        // ---------------------------------------------- Active Record BEGIN ----------------------------------------------
        [HttpGet]
        public IActionResult ProductsByCategory_ActiveRecord(
            [FromQuery] int categoryId,
            [FromQuery] List<string> selectedSizes = null,
            [FromQuery] List<string> selectedColors = null
        )
        {
            var products = ProductActiveRecord.GetProductsByCategory(categoryId, _connectionString);

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

            // Vytvoříme ViewModel pro zobrazení produktů a filtrů
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = categoryId,
                CategoryName = products.Count > 0 ? products[0].Name : "Neznámá kategorie",
                Products = products
                    .Select(p => new Product
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImgUrl = p.ImgUrl,
                        Variants = p.Variants,
                    })
                    .ToList(),
                AvailableSizes = selectedSizes,
                SelectedSizes = selectedSizes,
                AvailableColors = selectedColors,
                SelectedColors = selectedColors,
            };

            return View(viewModel);
        }

        // ---------------------------------------------- Active Record END ----------------------------------------------
        // ---------------------------------------------- Data Mapper BEGIN ----------------------------------------------
        [HttpGet]
        public IActionResult ProductsByCategory_DataMapper(
            [FromQuery] int categoryId,
            [FromQuery] List<string> selectedSizes = null,
            [FromQuery] List<string> selectedColors = null
        )
        {
            var productMapper = new ProductMapper(_connectionString);
            var products = productMapper.GetProductsByCategory(categoryId);

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

            // Vytvoříme ViewModel pro zobrazení produktů a filtrů
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = categoryId,
                CategoryName = products.Count > 0 ? products[0].Name : "Neznámá kategorie",
                Products = products,
                AvailableSizes = selectedSizes,
                SelectedSizes = selectedSizes,
                AvailableColors = selectedColors,
                SelectedColors = selectedColors,
            };

            return View(viewModel);
        }
        // ---------------------------------------------- Data Mapper END ----------------------------------------------
    }
}
