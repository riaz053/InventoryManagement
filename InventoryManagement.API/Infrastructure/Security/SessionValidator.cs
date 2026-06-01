using InventoryManagement.API.Data;

namespace InventoryManagement.API.Infrastructure.Security
{
    public class SessionValidator
    {
        private readonly ApplicationDbContext _context;

        public SessionValidator(ApplicationDbContext context)
        {
            _context = context;
        }

        // public bool IsValid(string userId, string sessionId)
        // {
        //     var user = _context.Users
        //         .FirstOrDefault(x => x.Id.ToString() == userId);

        //     if (user == null)
        //         return false;

        //     return user.CurrentSessionId == sessionId;
        // }
        public bool IsValid(string userId, string sessionId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId))
                return false;

            var user = _context.Users
                .FirstOrDefault(x => x.Id.ToString() == userId);

            if (user == null)
                return false;

            return user.CurrentSessionId == sessionId;
        }
    }
}