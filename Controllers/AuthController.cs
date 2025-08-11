using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZocoApp.Data;
using ZocoApp.DTOs;
using ZocoApp.Helpers;
using ZocoApp.Models;

namespace ZocoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            var token = _jwtHelper.GenerateJwtToken(user);

           
            var log = new SessionLog
            {
                UserId = user.Id,
                StartTime = DateTime.UtcNow
            };
            _context.SessionLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { token, sessionLogId = log.Id });
        }

        
        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hash == storedHash;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (exists)
                return BadRequest("El email ya está registrado.");

            var hashedPassword = HashPassword(registerDto.Password);

            var newUser = new User
            {
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                Role = registerDto.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente.");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            
            var lastOpen = await _context.SessionLogs
                .Where(s => s.UserId == userId && s.EndTime == null)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();

            if (lastOpen == null)
                return NotFound("No hay sesión abierta para cerrar.");

            lastOpen.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("Sesión cerrada.");
        }

    }
}
