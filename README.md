# VIS - Projekt E-commerce / E-shop

Toto je projekt do předmětu Vývoj Informačních Systémů
Autor: Jan Bojko 3. Ročník, obor Informatika.

Téma - E-shop / E-commerce s Muay Thai vybavením.

## Jaký tech stack projekt používá?

V tomto projektu jsem se rozhodl kromě povinného Bootstrapu použít další technologie pro efektivní vývoj.

-   [C#](https://learn.microsoft.com/dotnet/csharp/)
-   [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)
-   [ADO.NET](https://learn.microsoft.com/dotnet/framework/data/adonet/ado-net-overview)
-   [TailwindCSS](https://tailwindcss.com/)
-   [PostgreSQL](https://www.postgresql.org/)
-   [NPM](https://www.npmjs.com/)
-   [JavaScript](https://www.javascript.com/)

## Jak spustit lokálně

Chcete-li projekt spustit, je třeba mít nainstalovaný [PostgreSQL](https://www.postgresql.org/download/) a [Node.js](https://nodejs.org/en/download/prebuilt-installer) a vytvořenou databázi jménem `vis_projekt`, po vytvoření databáze je třeba vytvořit tabulky (a ideálně je naplnit daty), příkazy pro vytvoření tabulek najedete v `./DatabaseDefinition/db.pgsql` souboru.

Po vytvoření databáze následujte tento postup:

-   Ideálně databázi naplňte mockup daty
-   Naklonujte repozitář
-   V souboru `./appsettings.json` upravte hodnotu `"DefaultConnection"` na connection string odpovídající nastavení Vašeho Postgres serveru
-   Spusttě `npm install` příkaz ve složce projektu pro nainstalování node.js balíčků
-   Spusttě `npm run dev` příkaz v kořenovém adresáři projektu pro spuštění aplikace
-   Aplikace nyní běží na http://localhost:5280
