using System;
using Npgsql;

namespace Projekt.Services
{
    public class ProductRowGateway
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }

        private readonly string _connectionString;

        public ProductRowGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Load(int productId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT ""productid"", ""name"", ""description"", ""price"", ""imgurl""
                               FROM ""products""
                               WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("productId", productId);

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
            }
        }

        public void Save()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    ProductId == 0
                        ? @"INSERT INTO ""products"" (""name"", ""description"", ""price"", ""imgurl"")
                        VALUES (@name, @description, @price, @imgurl)"
                        : @"UPDATE ""products"" SET ""name"" = @name, ""description"" = @description, ""price"" = @price, ""imgurl"" = @imgurl
                        WHERE ""productid"" = @productId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("name", Name);
                    command.Parameters.AddWithValue("description", Description);
                    command.Parameters.AddWithValue("price", Price);
                    command.Parameters.AddWithValue("imgurl", ImgUrl);

                    if (ProductId != 0)
                    {
                        command.Parameters.AddWithValue("productId", ProductId);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
