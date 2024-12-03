using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.DataMappers;
using Projekt.Models;
using Projekt.Models.ViewModels;
using Projekt.Services;

namespace Projekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager _userManager;

        // Konstruktor s injektovaným UserManagerem
        public AccountController(UserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult GetUserInfo()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
            {
                System.Console.WriteLine();
                return Json(new { IsAuthenticated = false });
            }

            var firstName = User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Json(
                new
                {
                    IsAuthenticated = true,
                    UserName = User.Identity.Name, // Email nebo jiný identifikátor
                    FirstName = firstName,
                    LastName = lastName,
                    Role = role,
                }
            );
        }

        [Authorize]
        public IActionResult Details()
        {
            return View();
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Zpracování přihlášení (např. ověření uživatele)
                bool isValidUser = _userManager.AuthenticateUser(model.Email, model.Password);

                if (isValidUser)
                {
                    var user = _userManager.GetUserByEmail(model.Email);

                    Console.WriteLine(user);

                    // Vytvoření claims pro přihlášeného uživatele
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email), // Nastavení jména
                        new Claim("FirstName", user.FirstName),
                        new Claim("LastName", user.LastName),
                        new Claim(ClaimTypes.Email, model.Email), // Nastavení emailu
                        new Claim(ClaimTypes.Role, "Customer"), // Přidání role (např. "Customer")
                    };

                    // Nastavení identity a principal
                    var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                    var principal = new ClaimsPrincipal(identity);

                    // Přihlášení uživatele pomocí SignInAsync
                    await HttpContext.SignInAsync(principal);

                    // Přesměrování na domovskou stránku
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.ErrorMessage = "Neplatné přihlašovací údaje.";
                }
            }

            // Znovu zobrazit formulář s chybou
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    model.ErrorMessage = "Hesla se neshodují.";
                    return View(model);
                }

                if (_userManager.CheckEmailExists(model.Email))
                {
                    model.ErrorMessage = "Tento e-mail je již zaregistrován.";
                    return View(model);
                }

                // Uložení uživatele do databáze
                _userManager.SaveUserToDatabase(model);

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Přesměrování na domovskou stránku
        }
    }
}
