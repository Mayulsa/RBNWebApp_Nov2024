using BloodBankApp_Nov2024.Data;
using BloodBankApp_Nov2024.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBankApp_Nov2024.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(string username, string email, string password, string role);
        Task<ApplicationUser> LoginUser(string email, string password);
    }
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUser(string username, string email, string password, string role)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return false;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new ApplicationUser
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicationUser> LoginUser(string email, string password)
        {


            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);



            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        //public async Task<bool> IsInRole(string role)
        //{
        //    var profile = await GetUserProfile();
        //    return profile?.Role == role;
        //}
    }
}
