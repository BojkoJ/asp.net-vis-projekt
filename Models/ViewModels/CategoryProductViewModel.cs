namespace Projekt.Models
{
    public class CategoryProductsViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
        public List<string> AvailableSizes { get; set; } // Seznam všech dostupných velikostí
        public List<string> SelectedSizes { get; set; } // Vybrané velikosti
        public List<string> AvailableColors { get; set; } // Seznam všech dostupných barev
        public List<string> SelectedColors { get; set; } // Vybrané barvy
        public List<string> AvailableBrands { get; set; } // Seznam všech dostupných značek
        public List<string> SelectedBrands { get; set; } // Vybrané značky
    }
}
