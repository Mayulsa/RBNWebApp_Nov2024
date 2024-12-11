//using Microsoft.Data.SqlClient;
//using System.Data;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MySql.Data.MySqlClient;
//using BloodBankApp_Nov2024.Class;
//using BloodBankApp_Nov2024.Components.Pages.Nevera;
//using Mysqlx.Crud;
//using Google.Protobuf.WellKnownTypes;
//using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using static Google.Protobuf.WellKnownTypes.Field.Types;
//using System.Threading.Channels;
//using System.Threading;

//namespace BloodBankApp_Nov2024.Class
//{
//    public class Crud
//    {
//        private readonly string _connectionString = "Server=DESKTOP-VN0KBGI;Database=BD_BloodBankApp_Nov2024;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";

//        public Crud(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        // Método para obtener una conexión a la base de datos
//        private MySqlConnection GetConnection()
//        {
//            return new MySqlConnection(_connectionString);
//        }

//        // Método para crear un registro
//        public async Task<int> CreateAsync(string name, int age)
//        {
//            const string sql = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age);";
//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@Name", name);
//                    command.Parameters.AddWithValue("@Age", age);
//                    return await command.ExecuteNonQueryAsync();
//                }
//            }
//        }

//        // Método para leer todos los registros
//        public async Task<List<User>> GetAllAsync()
//        {
//            const string sql = "SELECT * FROM Users;";
//            var users = new List<User>();

//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(sql, connection))
//                {
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            users.Add(new User
//                            {
//                                Id = reader.GetInt32("Id"),
//                                Name = reader.GetString("Name"),
//                                Age = reader.GetInt32("Age")
//                            });
//                        }
//                    }
//                }
//            }

//            return users;
//        }

//        // Método para leer un registro por ID
//        public async Task<User> GetByIdAsync(int id)
//        {
//            const string sql = "SELECT * FROM Users WHERE Id = @Id;";
//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@Id", id);
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        if (await reader.ReadAsync())
//                        {
//                            return new User
//                            {
//                                Id = reader.GetInt32("Id"),
//                                Name = reader.GetString("Name"),
//                                Age = reader.GetInt32("Age")
//                            };
//                        }
//                    }
//                }
//            }

//            return null;
//        }

//        // Método para actualizar un registro
//        public async Task<int> UpdateAsync(int id, string name, int age)
//        {
//            const string sql = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id;";
//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@Id", id);
//                    command.Parameters.AddWithValue("@Name", name);
//                    command.Parameters.AddWithValue("@Age", age);
//                    return await command.ExecuteNonQueryAsync();
//                }
//            }
//        }

//        // Método para eliminar un registro
//        public async Task<int> DeleteAsync(int id)
//        {
//            const string sql = "DELETE FROM Users WHERE Id = @Id;";
//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@Id", id);
//                    return await command.ExecuteNonQueryAsync();
//                }
//            }
//        }

//        // Método para ejecutar un procedimiento almacenado
//        public async Task<List<User>> ExecuteStoredProcedureAsync(string storedProcedureName, int ageThreshold)
//        {
//            var users = new List<User>();

//            using (var connection = GetConnection())
//            {
//                await connection.OpenAsync();
//                using (var command = new MySqlCommand(storedProcedureName, connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.Parameters.AddWithValue("@AgeThreshold", ageThreshold);

//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            users.Add(new User
//                            {
//                                Id = reader.GetInt32("Id"),
//                                Name = reader.GetString("Name"),
//                                Age = reader.GetInt32("Age")
//                            });
//                        }
//                    }
//                }
//            }

//            return users;
//        }
//    }

//    // Clase modelo que representa la tabla Users
//    public class User
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public int Age { get; set; }
//    }
//}
