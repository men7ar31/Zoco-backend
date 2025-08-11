using Microsoft.EntityFrameworkCore;
using ZocoApp.Data;
using ZocoApp.Models;
using ZocoApp.Services.Interfaces;

namespace ZocoApp.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _ctx;
        public UsersService(AppDbContext ctx) => _ctx = ctx;

        public async Task<User?> GetByIdAsync(int id) => await _ctx.Users.FindAsync(id);

        public async Task<bool> EmailTakenAsync(string email, int? excludeId = null)
            => await _ctx.Users.AnyAsync(u => u.Email == email && (!excludeId.HasValue || u.Id != excludeId));

        public async Task<User> CreateAsync(string email, string passwordHash, string role)
        {
            var u = new User { Email = email, PasswordHash = passwordHash, Role = role };
            _ctx.Users.Add(u);
            await _ctx.SaveChangesAsync();
            return u;
        }

        public Task SaveAsync() => _ctx.SaveChangesAsync();
    }
}
