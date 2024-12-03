using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class OrderSummaryViewModel
    {
        // Rekapitulace produktů v objednávce
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        // Celková cena objednávky
        [Display(Name = "Celková cena")]
        public decimal TotalPrice { get; set; }

        // Zvolený způsob dopravy
        [Display(Name = "Způsob dopravy")]
        public string? ShippingMethod { get; set; }

        // Pokud je Z-box nebo výdejní místo
        [Display(Name = "Pickup point")]
        public string? PickupPoint { get; set; }

        // Pokud je doručovací adresa
        [Display(Name = "Doručovací adresa")]
        public string? DeliveryAddress { get; set; }

        // Zvolený způsob platby
        [Display(Name = "Způsob platby")]
        public string? PaymentMethod { get; set; }

        // Uživatelské údaje
        [Display(Name = "Jméno")]
        public string FirstName { get; set; }

        [Display(Name = "Příjmení")]
        public string LastName { get; set; }

        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Neplatná e-mailová adresa")]
        public string Email { get; set; }
    }
}
