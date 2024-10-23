using System.Collections.Generic;
using Npgsql; // pro PostgreSQL
using Projekt.Models;

namespace Projekt.Services
{
    public class ProductManager
    {
        private readonly string _connectionString;

        public ProductManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetProductsByCategory(int categoryId, int limit = 6, int offset = 0)
        {
            List<Product> products = new List<Product>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT p.""productid"", p.""name"", p.""description"", p.""price"", p.""imgurl"", c.""categoryid"", c.""name"" AS CategoryName
              FROM ""products"" p
              JOIN ""categories"" c ON p.""categoryid"" = c.""categoryid""
              WHERE p.""categoryid"" = @categoryid
              LIMIT @Limit OFFSET @Offset";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("CategoryId", categoryId);
                    command.Parameters.AddWithValue("Limit", limit);
                    command.Parameters.AddWithValue("Offset", offset);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImgUrl = reader.GetString(4),
                                Category = new Category
                                {
                                    CategoryId = reader.GetInt32(5),
                                    Name = reader.GetString(6),
                                },
                                Variants = new List<ProductVariant>(), // Inicializace prázdné kolekce variant
                            };

                            products.Add(product);
                        }
                    }
                }
            }

            // Načteme varianty pro každý produkt zvlášť
            foreach (var product in products)
            {
                product.Variants = GetVariantsByProductId(product.ProductId);
            }

            return products;
        }

        public List<Product> GetProductsByCategoryFilter(
            int categoryId,
            List<string> selectedSizes = null,
            List<string> selectedColors = null,
            int limit = 6,
            int offset = 0
        )
        {
            List<Product> products = new List<Product>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Základní SQL dotaz s GROUP BY na produkt
                string sql =
                    @"SELECT p.""productid"", p.""name"", p.""description"", p.""price"", p.""imgurl"", c.""categoryid"", c.""name"" AS CategoryName
              FROM ""products"" p
              JOIN ""categories"" c ON p.""categoryid"" = c.""categoryid""
              JOIN ""productvariants"" v ON p.""productid"" = v.""productid""
              WHERE p.""categoryid"" = @categoryid";

                // Dynamicky přidáme filtr pro velikosti, pokud je nastaven
                if (selectedSizes != null && selectedSizes.Count > 0)
                {
                    sql += @" AND v.""size"" = ANY(@selectedSizes)";
                }

                // Dynamicky přidáme filtr pro barvy, pokud je nastaven
                if (selectedColors != null && selectedColors.Count > 0)
                {
                    sql += @" AND v.""color"" = ANY(@selectedColors)";
                }

                // Skupinové vyhledávání podle produktů
                sql += @" GROUP BY p.""productid"", c.""categoryid""";

                // Přidáme LIMIT a OFFSET
                sql += " LIMIT @Limit OFFSET @Offset";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    // Přidáme parametry
                    command.Parameters.AddWithValue("categoryid", categoryId);
                    command.Parameters.AddWithValue("Limit", limit);
                    command.Parameters.AddWithValue("Offset", offset);

                    // Přidáme parametry pro filtry, pokud jsou nastaveny
                    if (selectedSizes != null && selectedSizes.Count > 0)
                    {
                        command.Parameters.AddWithValue("selectedSizes", selectedSizes.ToArray());
                    }
                    if (selectedColors != null && selectedColors.Count > 0)
                    {
                        command.Parameters.AddWithValue("selectedColors", selectedColors.ToArray());
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImgUrl = reader.GetString(4),
                                Category = new Category
                                {
                                    CategoryId = reader.GetInt32(5),
                                    Name = reader.GetString(6),
                                },
                                Variants = new List<ProductVariant>(), // Inicializace prázdné kolekce variant
                            };

                            products.Add(product);
                        }
                    }
                }
            }

            // Načteme varianty pro každý produkt zvlášť
            foreach (var product in products)
            {
                product.Variants = GetVariantsByProductId(product.ProductId);
            }

            return products;
        }

        public List<ProductVariant> GetVariantsByProductId(int productId)
        {
            List<ProductVariant> variants = new List<ProductVariant>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""productvariantid"", ""size"", ""color"", ""stockquantity"", ""productid""
              FROM ""productvariants""
              WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("productId", productId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            variants.Add(
                                new ProductVariant
                                {
                                    ProductVariantId = reader.GetInt32(0),
                                    Size = reader.GetString(1),
                                    Color = reader.GetString(2),
                                    StockQuantity = reader.GetInt32(3),
                                    ProductId = reader.GetInt32(4),
                                }
                            );
                        }
                    }
                }
            }

            return variants;
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT ""categoryid"", ""name"" FROM ""categories""";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(
                                new Category
                                {
                                    CategoryId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                }
                            );
                        }
                    }
                }
            }

            return categories;
        }

        public Category GetCategoryById(int categoryId)
        {
            Category category = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""categoryid"", ""name"" FROM ""categories"" WHERE ""categoryid"" = @categoryId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            category = new Category
                            {
                                CategoryId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            };
                        }
                    }
                }
            }

            return category;
        }
    }
}
