using System.Collections.Generic;
using Npgsql; // pro PostgreSQL
using Projekt.Models;

namespace Projekt.Services
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetProductsByCategory(int categoryId, int limit = 3)
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
                                LIMIT @Limit";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("CategoryId", categoryId);
                    command.Parameters.AddWithValue("Limit", limit);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(
                                new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    ImgUrl = reader.GetString(4), // Načti nový sloupec
                                    Category = new Category
                                    {
                                        CategoryId = reader.GetInt32(5),
                                        Name = reader.GetString(6),
                                    },
                                }
                            );
                        }
                    }
                }
            }

            return products;
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
    }
}
