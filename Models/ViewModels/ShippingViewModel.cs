using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class ShippingViewModel
    {
        [Required(ErrorMessage = "Prosím vyberte způsob dopravy.")]
        [Display(Name = "Způsob dopravy")]
        public string ShippingMethod { get; set; }
    }
}
