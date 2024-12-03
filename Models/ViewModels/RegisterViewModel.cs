namespace Projekt.Models
{
    public class RegisterViewModel
    {
        // Povinné údaje pro registraci
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } // Pro ověření hesla

        // Chybová zpráva v případě chyby
        public string? ErrorMessage { get; set; } = string.Empty;
    }
}
