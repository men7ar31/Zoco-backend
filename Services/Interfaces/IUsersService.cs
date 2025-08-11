using ZocoApp.Models;

namespace ZocoApp.Services.Interfaces
{
    public interface IUsersService
    {
        Task<User?> GetByIdAsync(int id);
        Task<bool> EmailTakenAsync(string email, int? excludeId = null);
        Task<User> CreateAsync(string email, string passwordHash, string role);
        Task SaveAsync();
    }
}

