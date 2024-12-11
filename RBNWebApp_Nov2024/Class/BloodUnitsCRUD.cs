using Microsoft.Data.SqlClient;

namespace RBNWebApp_Nov2024.Class
{
    public class BloodUnitsCRUD
    {
        private readonly string _connectionString;

        public BloodUnitsCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Create a new blood unit
        public async Task<int> CreateAsync(BloodUnit bloodUnit)
        {
            const string sql = @"
                INSERT INTO BloodUnits (DrawerID, BloodType, Quantity, DonationDate, ExpiryDate, Status) 
                VALUES (@DrawerID, @BloodType, @Quantity, @DonationDate, @ExpiryDate, @Status);

                DECLARE @NewBloodUnitID INT = SCOPE_IDENTITY();

                -- Insert into BloodUnitsHistory
                INSERT INTO BloodUnitsHistory (BloodUnitID, UserID, OldStatus, NewStatus, Notes)
                VALUES (@NewBloodUnitID, @UserID, NULL, @Status, 'Initial creation');

                SELECT @NewBloodUnitID;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@DrawerID", bloodUnit.DrawerID);
            command.Parameters.AddWithValue("@BloodType", bloodUnit.BloodType);
            command.Parameters.AddWithValue("@Quantity", bloodUnit.Quantity);
            command.Parameters.AddWithValue("@DonationDate", bloodUnit.DonationDate);
            command.Parameters.AddWithValue("@ExpiryDate", bloodUnit.ExpiryDate);
            command.Parameters.AddWithValue("@Status", bloodUnit.Status ?? "Disponible");
            command.Parameters.AddWithValue("@UserID", bloodUnit.CurrentUserID); // Used for history tracking

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Get all blood units
        public async Task<List<BloodUnit>> GetAllAsync()
        {
            const string sql = @"
                SELECT BloodUnitID, DrawerID, BloodType, Quantity, DonationDate, ExpiryDate, Status 
                FROM BloodUnits;";

            var bloodUnits = new List<BloodUnit>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bloodUnits.Add(MapBloodUnitFromReader(reader));
            }

            return bloodUnits;
        }

        // Get blood units by UserID
        public async Task<List<BloodUnit>> GetAllByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT bu.BloodUnitID, bu.DrawerID, bu.BloodType, bu.Quantity, bu.DonationDate, bu.ExpiryDate, bu.Status
                FROM BloodUnits bu
                JOIN Drawers d ON bu.DrawerID = d.DrawerID
                JOIN Refrigerators r ON d.RefrigeratorID = r.RefrigeratorID
                WHERE r.UserID = @UserID;";

            var bloodUnits = new List<BloodUnit>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bloodUnits.Add(MapBloodUnitFromReader(reader));
            }

            return bloodUnits;
        }

        // Get blood units by DrawerID
        public async Task<List<BloodUnit>> GetAllByDrawerIdAsync(int drawerId)
        {
            const string sql = @"
                SELECT BloodUnitID, DrawerID, BloodType, Quantity, DonationDate, ExpiryDate, Status 
                FROM BloodUnits 
                WHERE DrawerID = @DrawerID;";

            var bloodUnits = new List<BloodUnit>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DrawerID", drawerId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bloodUnits.Add(MapBloodUnitFromReader(reader));
            }

            return bloodUnits;
        }

        // Get blood unit by ID
        public async Task<BloodUnit?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT BloodUnitID, DrawerID, BloodType, Quantity, DonationDate, ExpiryDate, Status 
                FROM BloodUnits 
                WHERE BloodUnitID = @Id;";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapBloodUnitFromReader(reader);
            }

            return null;
        }

        // Get blood units by blood type
        public async Task<List<BloodUnit>> GetByBloodTypeAsync(string bloodType)
        {
            const string sql = @"
                SELECT BloodUnitID, DrawerID, BloodType, Quantity, DonationDate, ExpiryDate, Status 
                FROM BloodUnits 
                WHERE BloodType = @BloodType;";

            var bloodUnits = new List<BloodUnit>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BloodType", bloodType);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bloodUnits.Add(MapBloodUnitFromReader(reader));
            }

            return bloodUnits;
        }

        // Update a blood unit with history tracking
        public async Task<bool> UpdateAsync(BloodUnit bloodUnit, int currentUserId)
        {
            const string sql = @"
                -- Get current status
                DECLARE @OldStatus VARCHAR(20);
                SELECT @OldStatus = Status 
                FROM BloodUnits 
                WHERE BloodUnitID = @Id;

                -- Update blood unit
                UPDATE BloodUnits 
                SET DrawerID = @DrawerID,
                    BloodType = @BloodType,
                    Quantity = @Quantity,
                    DonationDate = @DonationDate,
                    ExpiryDate = @ExpiryDate,
                    Status = @Status
                WHERE BloodUnitID = @Id;

                -- Insert history record if status changed
                IF @OldStatus <> @Status
                BEGIN
                    INSERT INTO BloodUnitsHistory (BloodUnitID, UserID, OldStatus, NewStatus, Notes)
                    VALUES (@Id, @UserID, @OldStatus, @Status, @Notes);
                END";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", bloodUnit.Id);
            command.Parameters.AddWithValue("@DrawerID", bloodUnit.DrawerID);
            command.Parameters.AddWithValue("@BloodType", bloodUnit.BloodType);
            command.Parameters.AddWithValue("@Quantity", bloodUnit.Quantity);
            command.Parameters.AddWithValue("@DonationDate", bloodUnit.DonationDate);
            command.Parameters.AddWithValue("@ExpiryDate", bloodUnit.ExpiryDate);
            command.Parameters.AddWithValue("@Status", bloodUnit.Status);
            command.Parameters.AddWithValue("@UserID", currentUserId);
            command.Parameters.AddWithValue("@Notes", bloodUnit.StatusChangeNotes ?? (object)DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Delete a blood unit
        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            const string sql = @"
        BEGIN TRANSACTION;
        BEGIN TRY
            -- Delete associated reservation history records
            DELETE FROM ReservationHistory 
            WHERE ReservationID IN (SELECT ReservationID FROM Reservations WHERE BloodUnitID = @Id);

            -- Delete associated reservations
            DELETE FROM Reservations 
            WHERE BloodUnitID = @Id;

            -- Insert final history record
            INSERT INTO BloodUnitsHistory (BloodUnitID, UserID, OldStatus, NewStatus, Notes)
            SELECT @Id, @UserID, Status, 'Deleted', 'Unit deleted from system'
            FROM BloodUnits
            WHERE BloodUnitID = @Id;

            -- Disable the constraint
            ALTER TABLE BloodUnitsHistory NOCHECK CONSTRAINT FK__BloodUnit__Blood__2A164134;

            -- Delete the blood unit
            DELETE FROM BloodUnits WHERE BloodUnitID = @Id;
            
            -- Re-enable the constraint
            ALTER TABLE BloodUnitsHistory CHECK CONSTRAINT FK__BloodUnit__Blood__2A164134;

            COMMIT TRANSACTION;
            RETURN 1;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH";

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UserID", currentUserId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Get history for a blood unit
        public async Task<List<BloodUnitHistory>> GetHistoryAsync(int bloodUnitId)
        {
            const string sql = @"
                SELECT h.HistoryID, h.BloodUnitID, h.UserID, h.OldStatus, h.NewStatus, 
                       h.ChangeDate, h.Notes, u.Name as UserName
                FROM BloodUnitsHistory h
                JOIN Users u ON h.UserID = u.UserID
                WHERE h.BloodUnitID = @BloodUnitID
                ORDER BY h.ChangeDate DESC;";

            var history = new List<BloodUnitHistory>();

            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BloodUnitID", bloodUnitId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                history.Add(new BloodUnitHistory
                {
                    Id = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                    BloodUnitID = reader.GetInt32(reader.GetOrdinal("BloodUnitID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    OldStatus = reader.IsDBNull(reader.GetOrdinal("OldStatus")) ? null : reader.GetString(reader.GetOrdinal("OldStatus")),
                    NewStatus = reader.GetString(reader.GetOrdinal("NewStatus")),
                    ChangeDate = reader.GetDateTime(reader.GetOrdinal("ChangeDate")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return history;
        }

        private BloodUnit MapBloodUnitFromReader(SqlDataReader reader)
        {
            return new BloodUnit
            {
                Id = reader.GetInt32(reader.GetOrdinal("BloodUnitID")),
                DrawerID = reader.GetInt32(reader.GetOrdinal("DrawerID")),
                BloodType = reader.GetString(reader.GetOrdinal("BloodType")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                DonationDate = reader.GetDateTime(reader.GetOrdinal("DonationDate")),
                ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
            };
        }
    }

    public class BloodUnit
    {
        public int Id { get; set; }
        public int DrawerID { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime DonationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "Disponible";
        // Additional properties for operations
        public int CurrentUserID { get; set; }
        public string? StatusChangeNotes { get; set; }
    }

    public class BloodUnitHistory
    {
        public int Id { get; set; }
        public int BloodUnitID { get; set; }
        public int UserID { get; set; }
        public string? OldStatus { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
        public string? Notes { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

}
