using Microsoft.Data.SqlClient;

namespace RBNWebApp_Nov2024.Class
{
    public class RefrigeratorCRUD
    {
        private readonly string _connectionString;

        public RefrigeratorCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Create a new refrigerator
        public async Task<int> CreateAsync(Refrigerator refrigerator)
        {
            const string sql = @"
                INSERT INTO Refrigerators (UserID, SerialNumber, Location, IsOperational) 
                VALUES (@UserID, @SerialNumber, @Location, @IsOperational);
                SELECT SCOPE_IDENTITY();";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@UserID", refrigerator.UserID);
            command.Parameters.AddWithValue("@SerialNumber", refrigerator.SerialNumber);
            command.Parameters.AddWithValue("@Location", refrigerator.Location ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsOperational", refrigerator.IsOperational);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Get all refrigerators
        public async Task<List<Refrigerator>> GetAllAsync()
        {
            const string sql = "SELECT RefrigeratorID, UserID, SerialNumber, Location, IsOperational FROM Refrigerators;";
            var refrigerators = new List<Refrigerator>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                refrigerators.Add(new Refrigerator
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                    Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                    IsOperational = reader.GetBoolean(reader.GetOrdinal("IsOperational")) ? 1 : 0
                });
            }

            return refrigerators;
        }

        // Get refrigerators by UserID
        public async Task<List<Refrigerator>> GetAllByUserIdAsync(int userId)
        {
            const string sql = "SELECT RefrigeratorID, UserID, SerialNumber, Location, IsOperational FROM Refrigerators WHERE UserID = @UserID;";
            var refrigerators = new List<Refrigerator>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserID", userId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                refrigerators.Add(new Refrigerator
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                    Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                    IsOperational = reader.GetBoolean(reader.GetOrdinal("IsOperational")) ? 1 : 0
                });
            }

            return refrigerators;
        }

        // Get refrigerator by ID
        public async Task<Refrigerator?> GetByIdAsync(int id)
        {
            const string sql = "SELECT RefrigeratorID, UserID, SerialNumber, Location, IsOperational FROM Refrigerators WHERE RefrigeratorID = @Id;";
            
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Refrigerator
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                    Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                    IsOperational = reader.GetBoolean(reader.GetOrdinal("IsOperational")) ? 1 : 0
                };
            }

            return null;
        }

        // Update a refrigerator
        public async Task<bool> UpdateAsync(Refrigerator refrigerator)
        {
            const string sql = @"
                UPDATE Refrigerators 
                SET UserID = @UserID,
                    SerialNumber = @SerialNumber,
                    Location = @Location,
                    IsOperational = @IsOperational
                WHERE RefrigeratorID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", refrigerator.Id);
            command.Parameters.AddWithValue("@UserID", refrigerator.UserID);
            command.Parameters.AddWithValue("@SerialNumber", refrigerator.SerialNumber);
            command.Parameters.AddWithValue("@Location", refrigerator.Location ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsOperational", refrigerator.IsOperational == 1);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Delete a refrigerator
        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Refrigerators WHERE RefrigeratorID = @Id;";
            
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public class Refrigerator
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string? SerialNumber { get; set; }
        public string? Location { get; set; }
        public int IsOperational { get; set; }
    }

}
