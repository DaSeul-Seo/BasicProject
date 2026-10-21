using Microsoft.EntityFrameworkCore;
using MiniDesk.Data;
using MiniDesk.Models;

namespace MiniDesk.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _db.Users
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _db.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
