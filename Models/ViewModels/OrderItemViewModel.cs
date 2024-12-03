using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class OrderItemViewModel
    {
        [Required]
        public int ProductVariantId { get; set; } // ID varianty produktu (velikost/barva)

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Potřebujete aspoň jeden kus.")]
        public int Quantity { get; set; } // Množství

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musí být větší než 0.")]
        public decimal Price { get; set; } // Cena za jednotku

        public string ProductName { get; set; } // Název produktu (volitelné, pro zobrazení v UI)

        public string Size { get; set; } // Velikost produktu (volitelné)

        public string Color { get; set; } // Barva produktu (volitelné)
    }
}
