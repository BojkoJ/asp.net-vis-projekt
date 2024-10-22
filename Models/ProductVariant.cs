namespace Projekt.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigační vlastnost
    }
}
