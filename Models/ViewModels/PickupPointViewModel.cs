using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class PickupPointViewModel
    {
        [Required(ErrorMessage = "Prosím vyberte výdejní místo nebo Z-box.")]
        [Display(Name = "Výdejní místo / Z-box")]
        public string SelectedPoint { get; set; }

        // Možné body k výběru (např. seznam Z-boxů nebo výdejních míst)
        public List<string> AvailablePoints { get; set; }

        public PickupPointViewModel()
        {
            // Inicializace prázdného seznamu, aby se předešlo NullReferenceException
            AvailablePoints = new List<string>();
        }
    }
}
