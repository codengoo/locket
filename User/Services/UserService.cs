using Locket.UserLocket.Contexts;

namespace Locket.UserLocket.Services
{
    public class UserService(LocketDbContext context)
    {
        private readonly LocketDbContext _context = context;

        public string[] GetUsersByIDs(string[] Ids)
        {
            var users = _context.Users
                                .Where(user => Ids.Contains(user.Id.ToString()))
                                .Select(user => user.Username)
                                .ToArray();

            return users;
        }

        public string? GetUserByID(string ID)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id.ToString() == ID);

            return user?.Username ?? null;
        }
    }
}
