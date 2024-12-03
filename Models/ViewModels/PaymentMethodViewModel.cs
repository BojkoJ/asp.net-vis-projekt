using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class PaymentMethodViewModel
    {
        [Required(ErrorMessage = "Prosím zvolte způsob platby.")]
        [Display(Name = "Způsob platby")]
        public string SelectedPaymentMethod { get; set; }

        public List<string> AvailablePaymentMethods { get; set; } =
            new List<string> { "Online platba kartou", "Bankovní převod", "Dobírka" };
    }
}
