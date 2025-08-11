using Microsoft.EntityFrameworkCore;
using ZocoApp.Data;
using ZocoApp.Models;
using BCrypt.Net;

namespace ZocoApp.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync(u => u.Email == "admin@zoco.com"))
        {
            var admin = new User
            {
                Email = "admin@zoco.com",
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };
            db.Users.Add(admin);
        }

        if (!await db.Users.AnyAsync(u => u.Email == "test@zoco.com"))
        {
            var user = new User
            {
                Email = "test@zoco.com",
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
            };
            db.Users.Add(user);
        }

        await db.SaveChangesAsync();
    }
}

