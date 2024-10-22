using System.Collections.Generic;
using Npgsql;

namespace Projekt.Models
{
    public class ProductRowGateway
    {
        private readonly string _connectionString;

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public List<ProductVariant> Variants { get; set; }

        public ProductRowGateway(string connectionString)
        {
            _connectionString = connectionString;
            Variants = new List<ProductVariant>();
        }

        public void Load(int productId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""productid"", ""name"", ""description"", ""price"", ""imgurl"" FROM ""products"" WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@productId", productId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProductId = reader.GetInt32(0);
                            Name = reader.GetString(1);
                            Description = reader.GetString(2);
                            Price = reader.GetDecimal(3);
                            ImgUrl = reader.GetString(4);
                        }
                    }
                }

                LoadVariants();
            }
        }

        private void LoadVariants()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""size"", ""color"", ""stockquantity"" FROM ""productvariants"" WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@productId", ProductId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Variants.Add(
                                new ProductVariant
                                {
                                    Size = reader.GetString(0),
                                    Color = reader.GetString(1),
                                    StockQuantity = reader.GetInt32(2),
                                }
                            );
                        }
                    }
                }
            }
        }

        public void Save()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"UPDATE ""products"" SET ""name"" = @name, ""description"" = @description, ""price"" = @price, ""imgurl"" = @imgUrl WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@productId", ProductId);
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@description", Description);
                    command.Parameters.AddWithValue("@price", Price);
                    command.Parameters.AddWithValue("@imgUrl", ImgUrl);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<ProductRowGateway> GetProductsByCategory(
            int categoryId,
            string connectionString
        )
        {
            List<ProductRowGateway> products = new List<ProductRowGateway>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""productid"", ""name"", ""description"", ""price"", ""imgurl"" FROM ""products"" WHERE ""categoryid"" = @categoryId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductRowGateway(connectionString)
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
