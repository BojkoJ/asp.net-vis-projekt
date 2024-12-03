using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Projekt.Database; // Přidáno pro přístup k DatabaseConnection
using Projekt.Services;

namespace Projekt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Získání connection stringu
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            // Přidání služby session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Nastavte čas vypršení session
                options.Cookie.HttpOnly = true; // Zabezpečení cookies
                options.Cookie.IsEssential = true; // Session cookies jsou základní
            });

            // Inicializace databázového připojení
            DatabaseConnection.Initialize(connectionString);

            // Registrace ProductManager s connection stringem
            builder.Services.AddSingleton(new ProductManager(connectionString));

            // Registrace UserManager s connection stringem
            builder.Services.AddSingleton(new UserManager(connectionString));

            // Registrace OrderManager s connection stringem
            builder.Services.AddSingleton(new OrderManager(connectionString));

            // MVC a další služby
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            builder
                .Services.AddAuthentication("Cookies")
                .AddCookie(
                    "Cookies",
                    options =>
                    {
                        options.LoginPath = "/Account/Login"; // Cesta pro přihlášení
                        options.LogoutPath = "/Account/Logout"; // Cesta pro odhlášení
                    }
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
