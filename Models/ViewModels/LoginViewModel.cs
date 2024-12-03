namespace Projekt.Models
{
    public class LoginViewModel
    {
        // Email zadaný uživatelem
        public string Email { get; set; }

        // Heslo zadané uživatelem
        public string Password { get; set; }

        // Zapamatování uživatele na zařízení (volitelné)
        public bool RememberMe { get; set; }

        // Případná chybová zpráva při neúspěšném přihlášení
        public string? ErrorMessage { get; set; } = string.Empty;
    }
}
