using Microsoft.Data.SqlClient;

namespace RBNWebApp_Nov2024.Class
{
    public class ReservationCRUD
    {
        private readonly string _connectionString;

        public ReservationCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Create a new reservation
        public async Task<int> CreateAsync(Reservation reservation)
        {
            const string sql = @"
                INSERT INTO Reservations (BloodUnitID, UserID, RequestDate, Status, Notes) 
                VALUES (@BloodUnitID, @UserID, @RequestDate, @Status, @Notes);
                
                -- Insert into ReservationHistory
                DECLARE @NewReservationID INT = SCOPE_IDENTITY();
                INSERT INTO ReservationHistory (ReservationID, UserID, Status, ChangeDate)
                VALUES (@NewReservationID, @UserID, @Status, GETDATE());
                
                SELECT @NewReservationID;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@BloodUnitID", reservation.BloodUnitID);
            command.Parameters.AddWithValue("@UserID", reservation.UserID);
            command.Parameters.AddWithValue("@RequestDate", reservation.RequestDate ?? DateTime.Now);
            command.Parameters.AddWithValue("@Status", reservation.Status ?? "Pendiente");
            command.Parameters.AddWithValue("@Notes", reservation.Notes ?? (object)DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Get all reservations
        public async Task<List<Reservation>> GetAllAsync()
        {
            const string sql = @"
                SELECT r.ReservationID, r.BloodUnitID, r.UserID, r.RequestDate, r.Status, r.Notes,
                       b.BloodType, u.Name as UserName
                FROM Reservations r
                JOIN BloodUnits b ON r.BloodUnitID = b.BloodUnitID
                JOIN Users u ON r.UserID = u.UserID;";

            var reservations = new List<Reservation>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reservations.Add(new Reservation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                    BloodUnitID = reader.GetInt32(reader.GetOrdinal("BloodUnitID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    BloodType = reader.GetString(reader.GetOrdinal("BloodType")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return reservations;
        }

        // Get reservations by UserID
        public async Task<List<Reservation>> GetAllByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT r.ReservationID, r.BloodUnitID, r.UserID, r.RequestDate, r.Status, r.Notes,
                       b.BloodType, u.Name as UserName
                FROM Reservations r
                JOIN BloodUnits b ON r.BloodUnitID = b.BloodUnitID
                JOIN Users u ON r.UserID = u.UserID
                WHERE r.UserID = @UserID;";

            var reservations = new List<Reservation>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reservations.Add(new Reservation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                    BloodUnitID = reader.GetInt32(reader.GetOrdinal("BloodUnitID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    BloodType = reader.GetString(reader.GetOrdinal("BloodType")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return reservations;
        }

        // Get reservation by ID
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT r.ReservationID, r.BloodUnitID, r.UserID, r.RequestDate, r.Status, r.Notes,
                       b.BloodType, u.Name as UserName
                FROM Reservations r
                JOIN BloodUnits b ON r.BloodUnitID = b.BloodUnitID
                JOIN Users u ON r.UserID = u.UserID
                WHERE r.ReservationID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Reservation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                    BloodUnitID = reader.GetInt32(reader.GetOrdinal("BloodUnitID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    BloodType = reader.GetString(reader.GetOrdinal("BloodType")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                };
            }

            return null;
        }

        // Update a reservation
        public async Task<bool> UpdateAsync(Reservation reservation)
        {
            const string sql = @"
                UPDATE Reservations 
                SET Status = @Status,
                    Notes = @Notes,
                    AcceptanceTime = @AcceptanceTime
                WHERE ReservationID = @Id;

                -- Insert into ReservationHistory
                INSERT INTO ReservationHistory (ReservationID, UserID, Status, ChangeDate)
                VALUES (@Id, @UserID, @Status, GETDATE());";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", reservation.Id);
            command.Parameters.AddWithValue("@Status", reservation.Status);
            command.Parameters.AddWithValue("@Notes", reservation.Notes ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UserID", reservation.UserID);
            command.Parameters.AddWithValue("@AcceptanceTime",reservation.AcceptanceTime ?? (object)DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Delete a reservation
        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                DELETE FROM ReservationHistory WHERE ReservationID = @Id;
                DELETE FROM Reservations WHERE ReservationID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Get reservation history
        public async Task<List<ReservationHistory>> GetHistoryByReservationIdAsync(int reservationId)
        {
            const string sql = @"
                SELECT h.HistoryID, h.ReservationID, h.UserID, h.Status, h.ChangeDate,
                       u.Name as UserName
                FROM ReservationHistory h
                JOIN Users u ON h.UserID = u.UserID
                WHERE h.ReservationID = @ReservationID
                ORDER BY h.ChangeDate DESC;";

            var history = new List<ReservationHistory>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ReservationID", reservationId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                history.Add(new ReservationHistory
                {
                    Id = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                    ReservationID = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    ChangeDate = reader.GetDateTime(reader.GetOrdinal("ChangeDate")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return history;
        }
    }

    public class Reservation
    {
        public int Id { get; set; }
        public int BloodUnitID { get; set; }
        public int UserID { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? AcceptanceTime { get; set; }
        // Additional properties for joins
        public string? BloodType { get; set; }
        public string? UserName { get; set; }
    }

    public class ReservationHistory
    {
        public int Id { get; set; }
        public int ReservationID { get; set; }
        public int UserID { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
        public string? UserName { get; set; }
    }

}
