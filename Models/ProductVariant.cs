namespace Projekt.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; } // Primární klíč
        public string Size { get; set; } // Velikost, nebo délka produktu
        public string Color { get; set; } // Barva varianty
        public int StockQuantity { get; set; } // Počet kusů na skladě
        public int ProductId { get; set; } // Cizí klíč na Product
        public Product Product { get; set; } // Navigační vlastnost pro vztah s produktem
        public decimal Price { get; set; } // Cena varianty
    }
}
