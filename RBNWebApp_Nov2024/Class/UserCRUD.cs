using Microsoft.Data.SqlClient;
using System.Data;

namespace RBNWebApp_Nov2024.Class
{
    public class UserCRUD
    {
        private readonly string _connectionString = "Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";

        public UserCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para obtener una conexión a la base de datos
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Create a new user
        public async Task<int> CreateAsync(User user)
        {
            const string sql = @"
                INSERT INTO Users (Name, Email, Provincia, Password, Role, Latitude, Longitude, GooglePlaceID, IsActive) 
                VALUES (@Name, @Email, @Provincia, dbo.HashPassword(@Password), @Role, @Latitude, @Longitude, @GooglePlaceID, @IsActive);
                SELECT SCOPE_IDENTITY();";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Provincia", "Not Specified"); // Default value or add to User class
            command.Parameters.AddWithValue("@Password", user.Password ?? string.Empty);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@Latitude", user.Latitude);
            command.Parameters.AddWithValue("@Longitude", user.Longitude);
            command.Parameters.AddWithValue("@GooglePlaceID", user.GooglePlaceID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", true); // Default to active

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Get all users
        public async Task<List<User>> GetAllAsync()
        {
            const string sql = @"
                SELECT UserID, Name, Email, Role, Latitude, Longitude, GooglePlaceID, IsActive 
                FROM Users;";

            var users = new List<User>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    Latitude = reader.GetDecimal(reader.GetOrdinal("Latitude")),
                    Longitude = reader.GetDecimal(reader.GetOrdinal("Longitude")),
                    GooglePlaceID = reader.IsDBNull(reader.GetOrdinal("GooglePlaceID")) ? null : reader.GetString(reader.GetOrdinal("GooglePlaceID"))
                });
            }

            return users;
        }

        // Get user by ID
        public async Task<User?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT UserID, Name, Email, Role, Latitude, Longitude, GooglePlaceID, IsActive 
                FROM Users 
                WHERE UserID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    Latitude = reader.GetDecimal(reader.GetOrdinal("Latitude")),
                    Longitude = reader.GetDecimal(reader.GetOrdinal("Longitude")),
                    GooglePlaceID = reader.IsDBNull(reader.GetOrdinal("GooglePlaceID")) ? null : reader.GetString(reader.GetOrdinal("GooglePlaceID"))
                };
            }

            return null;
        }

        // Get user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT UserID, Name, Email, Role, Latitude, Longitude, GooglePlaceID, IsActive 
                FROM Users 
                WHERE Email = @Email;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    Latitude = reader.GetDecimal(reader.GetOrdinal("Latitude")),
                    Longitude = reader.GetDecimal(reader.GetOrdinal("Longitude")),
                    GooglePlaceID = reader.IsDBNull(reader.GetOrdinal("GooglePlaceID")) ? null : reader.GetString(reader.GetOrdinal("GooglePlaceID"))
                };
            }

            return null;
        }

        // Update a user
        public async Task<bool> UpdateAsync(User user)
        {
            const string sql = @"
                UPDATE Users 
                SET Name = @Name,
                    Email = @Email,
                    Role = @Role,
                    Latitude = @Latitude,
                    Longitude = @Longitude,
                    GooglePlaceID = @GooglePlaceID
                WHERE UserID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@Latitude", user.Latitude);
            command.Parameters.AddWithValue("@Longitude", user.Longitude);
            command.Parameters.AddWithValue("@GooglePlaceID", user.GooglePlaceID ?? (object)DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Update user password
        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            const string sql = @"
                UPDATE Users 
                SET Password = dbo.HashPassword(@Password)
                WHERE UserID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@Password", newPassword);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Delete a user (or deactivate)
        public async Task<bool> DeleteAsync(int id)
        {
            // Instead of actually deleting, we'll set IsActive to false
            const string sql = "UPDATE Users SET IsActive = 0 WHERE UserID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<(User? user, string message, bool success)> LoginAsync(string email, string password)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_Login", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);

            // Add output parameter for message
            var messageParam = new SqlParameter
            {
                ParameterName = "@Message",
                SqlDbType = SqlDbType.VarChar,
                Size = 100,
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(messageParam);

            try
            {
                using var reader = await command.ExecuteReaderAsync();

                // If we have results, read the user data
                if (reader.HasRows && await reader.ReadAsync())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Provincia = reader.GetString(reader.GetOrdinal("Provincia")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        Latitude = reader.GetDecimal(reader.GetOrdinal("Latitude")),
                        Longitude = reader.GetDecimal(reader.GetOrdinal("Longitude"))
                    };

                    return (user, messageParam.Value?.ToString() ?? "Login successful", true);
                }

                // If no results, return the error message
                return (null, messageParam.Value?.ToString() ?? "Login failed", false);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                return (null, "An error occurred during login", false);
            }
        }



    }

    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Provincia { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? GooglePlaceID { get; set; }
    }

}
