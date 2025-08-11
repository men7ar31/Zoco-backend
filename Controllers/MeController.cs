using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZocoApp.Data;
using ZocoApp.DTOs;

namespace ZocoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MeController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<MeDto>> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (u is null) return NotFound();

            return Ok(new MeDto { Id = u.Id, Email = u.Email, Role = u.Role });
        }

        [HttpPut]
        public async Task<IActionResult> Update(MeUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Users.FindAsync(userId);
            if (u is null) return NotFound();

            // validar email duplicado
            var taken = await _context.Users.AnyAsync(x => x.Email == dto.Email && x.Id != userId);
            if (taken) return BadRequest("Ya existe un usuario con ese email.");

            u.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                u.PasswordHash = Hash(dto.NewPassword);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
