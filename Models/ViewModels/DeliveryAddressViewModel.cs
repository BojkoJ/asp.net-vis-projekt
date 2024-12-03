using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class DeliveryAddressViewModel
    {
        [Required(ErrorMessage = "Prosím zadejte ulici a číslo popisné.")]
        [Display(Name = "Ulice a číslo popisné")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "Prosím zadejte město.")]
        [Display(Name = "Město")]
        public string City { get; set; }

        [Required(ErrorMessage = "Prosím zadejte PSČ.")]
        [Display(Name = "PSČ")]
        [RegularExpression(@"\d{5}", ErrorMessage = "PSČ musí mít 5 číslic.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Prosím zadejte stát.")]
        [Display(Name = "Stát")]
        public string Country { get; set; }

        public string FullAddress
        {
            get { return $"{StreetAddress}, {City}, {PostalCode}, {Country}"; }
        }
    }
}
