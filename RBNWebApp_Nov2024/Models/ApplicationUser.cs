namespace BloodBankApp_Nov2024.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // AdminBloodBank or UserBloodBank
    }
}
