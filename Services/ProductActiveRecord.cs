using System.Collections.Generic;
using Npgsql;

namespace Projekt.Models
{
    public class ProductActiveRecord
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        private readonly string _connectionString;

        public ProductActiveRecord(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Najdi produkt podle ID
        public static ProductActiveRecord Find(int productId, string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM products WHERE productid = @productId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("productId", productId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var product = new ProductActiveRecord(connectionString)
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImgUrl = reader.GetString(4),
                            };
                            product.LoadVariants(); // načteme varianty pro produkt
                            return product;
                        }
                    }
                }
            }
            return null;
        }

        // Načíst varianty produktu
        public void LoadVariants()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM productvariants WHERE productid = @productId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("productId", ProductId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Variants.Add(
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
        }

        // Uložit nebo aktualizovat produkt
        public void Save()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                if (ProductId == 0) // nový produkt
                {
                    string sql =
                        @"INSERT INTO products (name, description, price, imgurl) 
                                   VALUES (@name, @description, @price, @imgurl) 
                                   RETURNING productid";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("name", Name);
                        command.Parameters.AddWithValue("description", Description);
                        command.Parameters.AddWithValue("price", Price);
                        command.Parameters.AddWithValue("imgurl", ImgUrl);
                        ProductId = (int)command.ExecuteScalar(); // načteme nové ID
                    }
                }
                else // aktualizace produktu
                {
                    string sql =
                        @"UPDATE products SET name = @name, description = @description, price = @price, imgurl = @imgurl WHERE productid = @productId";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("name", Name);
                        command.Parameters.AddWithValue("description", Description);
                        command.Parameters.AddWithValue("price", Price);
                        command.Parameters.AddWithValue("imgurl", ImgUrl);
                        command.Parameters.AddWithValue("productId", ProductId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Smazání produktu
        public void Delete()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"DELETE FROM products WHERE productid = @productId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("productId", ProductId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Statická metoda pro získání produktů podle kategorie
        public static List<ProductActiveRecord> GetProductsByCategory(
            int categoryId,
            string connectionString
        )
        {
            List<ProductActiveRecord> products = new List<ProductActiveRecord>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM products WHERE categoryid = @categoryId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("categoryId", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductActiveRecord(connectionString)
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImgUrl = reader.GetString(4),
                            };
                            product.LoadVariants();
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }
    }
}
