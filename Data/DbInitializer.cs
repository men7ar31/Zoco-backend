using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ZocoApp.Models;

namespace ZocoApp.Data
{
    public static class DbInitializer
    {
        public static void Seed(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ctx.Database.Migrate();


            if (!ctx.Users.Any(u => u.Role == "Admin"))
            {
                ctx.Users.Add(new User
                {
                    Email = "admin@zoco.com",
                    PasswordHash = Hash("admin123"),
                    Role = "Admin"
                });
                ctx.SaveChanges();
            }
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
