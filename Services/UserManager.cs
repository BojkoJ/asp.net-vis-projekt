using System;
using System.Collections.Generic;
using Npgsql; // Pro PostgreSQL
using Projekt.Models;

namespace Projekt.Services
{
    public class UserManager
    {
        private readonly string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CheckEmailExists(string email)
        {
            bool emailExists = false;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = @"SELECT 1 FROM ""users"" WHERE ""email"" = @Email LIMIT 1";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("Email", email);

                        using (var reader = command.ExecuteReader())
                        {
                            emailExists = reader.HasRows;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in CheckEmailExists: " + ex.Message);
                }
            }

            return emailExists;
        }

        public void SaveUserToDatabase(RegisterViewModel model)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string sql =
                        @"
                        INSERT INTO ""users"" (""firstname"", ""lastname"", ""email"", ""password"", ""role"") 
                        VALUES (@FirstName, @LastName, @Email, @Password, @Role)";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("FirstName", model.FirstName);
                        command.Parameters.AddWithValue("LastName", model.LastName);
                        command.Parameters.AddWithValue("Email", model.Email);

                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        command.Parameters.AddWithValue("Password", hashedPassword);

                        command.Parameters.AddWithValue("Role", "Customer");

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in SaveUserToDatabase: " + ex.Message);
                }
            }
        }

        public bool AuthenticateUser(string email, string password)
        {
            bool isAuthenticated = false;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string sql =
                        @"SELECT ""password"" FROM ""users"" WHERE ""email"" = @Email LIMIT 1";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("Email", email);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader.GetString(0);

                                // Verify the password
                                isAuthenticated = BCrypt.Net.BCrypt.Verify(
                                    password,
                                    storedPasswordHash
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in AuthenticateUser: " + ex.Message);
                }
            }

            return isAuthenticated;
        }

        public User GetUserByEmail(string email)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"SELECT firstname, lastname, email, role FROM users WHERE email = @Email";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                FirstName = reader.GetString(0),
                                LastName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
