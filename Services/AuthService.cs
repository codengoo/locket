using locket.Contexts;
using locket.Models;
using Microsoft.EntityFrameworkCore;
namespace locket.Services
{
    public class AuthService(LocketDbContext context)
    {
        private readonly LocketDbContext _context = context;

        private async Task<bool> IsExist(string? username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task<Guid> InsertUserByUsername(string username, string? password, string? googleID = null, string? displayName = null)
        {
            bool isExisted = await IsExist(username);
            if (isExisted) throw new Exception("Username has been existed");

            string hash = BCrypt.Net.BCrypt.HashPassword(password ?? Guid.NewGuid().ToString());
            Guid Id = Guid.NewGuid();

            var user = new User
            {
                Username = username,
                GoogleID = googleID,
                DisplayName = displayName ?? username,
                Password = hash,
                Id = Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Id;
        }

        public async Task<Guid?> FindUserByUsername(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found.");
            var hash = user.Password;

            if (BCrypt.Net.BCrypt.Verify(password, hash))
            {
                return user.Id;
            }
            else
            {
                return null;
            }
        }

        public async Task<Guid?> FindUserByGoogleID(string googleID)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleID == googleID);
            
            if (user == null)
            {
                return null;
            } else
            {
                return user?.Id;
            }
        }
    }
}
