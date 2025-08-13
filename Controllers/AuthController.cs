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
            var email = loginDto.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            var token = _jwtHelper.GenerateJwtToken(user);

            var log = new SessionLog
            {
                UserId = user.Id,
                FechaInicio = DateTime.UtcNow
            };
            _context.SessionLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { token, sessionLogId = log.Id });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var email = registerDto.Email.Trim().ToLower();

            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);
            if (exists) return BadRequest("El email ya está registrado.");

            var newUser = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = string.IsNullOrWhiteSpace(registerDto.Role) ? "User" : registerDto.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("Usuario registrado exitosamente.");
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


            var lastOpen = await _context.SessionLogs
                .Where(s => s.UserId == userId && s.FechaFin == null)
                .OrderByDescending(s => s.FechaFin)
                .FirstOrDefaultAsync();

            if (lastOpen == null)
                return NotFound("No hay sesión abierta para cerrar.");

            lastOpen.FechaFin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("Sesión cerrada.");
        }

    }
}