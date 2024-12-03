using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Projekt.Models.ViewModels;

namespace Projekt.Services
{
    public class OrderManager
    {
        private readonly string _connectionString;

        public OrderManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateOrder(OrderSummaryViewModel model)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Získání ID uživatele na základě e-mailu
                        string getUserIdSql = "SELECT UserId FROM users WHERE Email = @Email";
                        int userId = 0;

                        using (
                            var command = new NpgsqlCommand(getUserIdSql, connection, transaction)
                        )
                        {
                            command.Parameters.AddWithValue("Email", model.Email);
                            var result = command.ExecuteScalar();
                            if (result == null || !int.TryParse(result.ToString(), out userId))
                            {
                                throw new Exception("Uživatel nebyl nalezen.");
                            }
                        }

                        // Získání ID všech produktů v objednávce
                        string getProductIdsSql =
                            "SELECT ProductId FROM products WHERE ProductId = @ProductId";
                        var productIds = new List<int>();

                        foreach (var item in model.Items)
                        {
                            using (
                                var command = new NpgsqlCommand(
                                    getProductIdsSql,
                                    connection,
                                    transaction
                                )
                            )
                            {
                                command.Parameters.AddWithValue("ProductId", item.ProductVariantId);
                                var result = command.ExecuteScalar();
                                if (
                                    result == null
                                    || !int.TryParse(result.ToString(), out var productId)
                                )
                                {
                                    throw new Exception("Produkt nebyl nalezen.");
                                }

                                productIds.Add(productId);
                            }
                        }

                        // Získání všech id variant všech produktů - List tuple (ProductId, ProductVariantId)
                        string getProductVariantIdsSql =
                            "SELECT ProductVariantId FROM productvariants WHERE ProductId = @ProductId";
                        var productVariantIds = new List<(int, int)>();

                        foreach (var productId in productIds)
                        {
                            using (
                                var command = new NpgsqlCommand(
                                    getProductVariantIdsSql,
                                    connection,
                                    transaction
                                )
                            )
                            {
                                command.Parameters.AddWithValue("ProductId", productId);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        productVariantIds.Add((productId, reader.GetInt32(0)));
                                    }
                                }
                            }
                        }

                        // Vytvoření objednávky
                        string insertOrderSql =
                            @"
                            INSERT INTO orders (orderdate, status, userid)
                            VALUES (@OrderDate, @Status, @UserId)
                            RETURNING orderid";
                        int orderId = 0;

                        using (
                            var command = new NpgsqlCommand(insertOrderSql, connection, transaction)
                        )
                        {
                            command.Parameters.AddWithValue("OrderDate", DateTime.UtcNow);
                            command.Parameters.AddWithValue("Status", "Pending");
                            command.Parameters.AddWithValue("UserId", userId);

                            orderId = (int)command.ExecuteScalar();
                        }

                        // Vložení položek objednávky
                        foreach (var item in model.Items)
                        {
                            string insertOrderItemSql =
                                @"
                                INSERT INTO orderitems (quantity, price, productvariantid, orderid)
                                VALUES (@Quantity, @Price, @ProductVariantId, @OrderId)";

                            using (
                                var command = new NpgsqlCommand(
                                    insertOrderItemSql,
                                    connection,
                                    transaction
                                )
                            )
                            {
                                command.Parameters.AddWithValue("Quantity", item.Quantity);
                                command.Parameters.AddWithValue("Price", item.Price);

                                // Použijeme vždy první variantu produktu
                                int firstProductVariantId = productVariantIds
                                    .Find(pv => pv.Item1 == item.ProductVariantId)
                                    .Item2;

                                command.Parameters.AddWithValue(
                                    "ProductVariantId",
                                    firstProductVariantId
                                );
                                command.Parameters.AddWithValue("OrderId", orderId);

                                command.ExecuteNonQuery();
                            }
                        }

                        // Commit
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
