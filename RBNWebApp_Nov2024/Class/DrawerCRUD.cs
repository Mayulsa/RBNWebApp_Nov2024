using Microsoft.Data.SqlClient;

namespace RBNWebApp_Nov2024.Class
{
    public class DrawerCRUD
    {
        private readonly string _connectionString;

        public DrawerCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Create a new drawer
        public async Task<int> CreateAsync(Drawer drawer)
        {
            const string sql = @"
                INSERT INTO Drawers (RefrigeratorID, DrawerNumber, Capacity) 
                VALUES (@RefrigeratorID, @DrawerNumber, @Capacity);
                SELECT SCOPE_IDENTITY();";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@RefrigeratorID", drawer.RefrigeratorID);
            command.Parameters.AddWithValue("@DrawerNumber", drawer.DrawerNumber);
            command.Parameters.AddWithValue("@Capacity", drawer.Capacity);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Get all drawers
        public async Task<List<Drawer>> GetAllAsync()
        {
            const string sql = "SELECT DrawerID, RefrigeratorID, DrawerNumber, Capacity FROM Drawers;";
            var drawers = new List<Drawer>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                drawers.Add(new Drawer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("DrawerID")),
                    RefrigeratorID = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    DrawerNumber = reader.GetInt32(reader.GetOrdinal("DrawerNumber")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
                });
            }

            return drawers;
        }

        // Get drawers by UserID
        public async Task<List<Drawer>> GetAllByUserIdAsync(int userId)
        {
            const string sql = "SELECT DrawerID, RefrigeratorID, DrawerNumber, Capacity FROM Drawers WHERE UserID = @UserID;";
            var drawers = new List<Drawer>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                drawers.Add(new Drawer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("DrawerID")),
                    RefrigeratorID = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    DrawerNumber = reader.GetInt32(reader.GetOrdinal("DrawerNumber")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
                });
            }

            return drawers;
        }

        // Get drawers by RefrigeratorID
        public async Task<List<Drawer>> GetAllByRefrigeratorIdAsync(int refrigeratorId)
        {
            const string sql = "SELECT DrawerID, RefrigeratorID, DrawerNumber, Capacity FROM Drawers WHERE RefrigeratorID = @RefrigeratorID;";
            var drawers = new List<Drawer>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@RefrigeratorID", refrigeratorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                drawers.Add(new Drawer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("DrawerID")),
                    RefrigeratorID = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    DrawerNumber = reader.GetInt32(reader.GetOrdinal("DrawerNumber")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
                });
            }

            return drawers;
        }

        // Get drawer by ID
        public async Task<Drawer?> GetByIdAsync(int id)
        {
            const string sql = "SELECT DrawerID, RefrigeratorID, DrawerNumber, Capacity FROM Drawers WHERE DrawerID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Drawer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("DrawerID")),
                    RefrigeratorID = reader.GetInt32(reader.GetOrdinal("RefrigeratorID")),
                    DrawerNumber = reader.GetInt32(reader.GetOrdinal("DrawerNumber")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
                };
            }

            return null;
        }

        // Update a drawer
        public async Task<bool> UpdateAsync(Drawer drawer)
        {
            const string sql = @"
                UPDATE Drawers 
                SET RefrigeratorID = @RefrigeratorID,
                    DrawerNumber = @DrawerNumber,
                    Capacity = @Capacity
                WHERE DrawerID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", drawer.Id);
            command.Parameters.AddWithValue("@RefrigeratorID", drawer.RefrigeratorID);
            command.Parameters.AddWithValue("@DrawerNumber", drawer.DrawerNumber);
            command.Parameters.AddWithValue("@Capacity", drawer.Capacity);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Delete a drawer
        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Drawers WHERE DrawerID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public class Drawer
    {
        public int Id { get; set; }
        public int RefrigeratorID { get; set; }
        public int DrawerNumber { get; set; }
        public int Capacity { get; set; }
    }

}
