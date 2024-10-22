namespace Projekt.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Navigační vlastnost
        public string ImgUrl { get; set; } // Nová vlastnost
    }
}
