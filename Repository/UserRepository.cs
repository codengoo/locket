using locket.Contexts;
using locket.Models;
using Microsoft.EntityFrameworkCore;

namespace locket.Repository
{
    public interface IRepository<T>
    {
        Task<bool> IsExist(string? username);

    }

    public class IReturnUser(User user)
    {
        Guid Id { get; } = user.Id;
        string Username { get; } = user.Username;
        string DisplayName { get; } = user.DisplayName;
    }

    public class UserRepository(LocketDbContext context) : IRepository<User>
    {
        private readonly LocketDbContext _context = context;
        public async Task<bool> IsExist(string? username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<IReturnUser> InsertUser(string username, string? password, string? googleID = null, string? displayName = null)
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

            return new IReturnUser(user);
        }
    }
}