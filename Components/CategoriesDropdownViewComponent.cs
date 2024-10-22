using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Projekt.Services;

namespace Projekt.Components
{
    public class CategoriesDropdownViewComponent : ViewComponent
    {
        private readonly DatabaseManager _db;

        public CategoriesDropdownViewComponent(DatabaseManager db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _db.GetCategories(); // Získání kategorií z databáze
            return View(categories); // Vrácení view s kategoriemi
        }
    }
}
