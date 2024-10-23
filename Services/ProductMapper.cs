using Npgsql;
using Projekt.Models;

namespace Projekt.DataMappers
{
    public class ProductMapper
    {
        private readonly string _connectionString;

        public ProductMapper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string sql =
                    @"SELECT p.""productid"", p.""name"", p.""description"", p.""price"", p.""imgurl"" 
                               FROM ""products"" p
                               WHERE p.""categoryid"" = @categoryId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);

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
                                Variants = new List<ProductVariant>(),
                            };

                            // Načteme varianty pro každý produkt
                            product.Variants = GetProductVariants(product.ProductId);
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public List<ProductVariant> GetProductVariants(int productId)
        {
            List<ProductVariant> variants = new List<ProductVariant>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string sql =
                    @"SELECT ""productvariantid"", ""size"", ""color"", ""stockquantity""
                               FROM ""productvariants""
                               WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@productId", productId);

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
                                }
                            );
                        }
                    }
                }
            }

            return variants;
        }
    }
}
