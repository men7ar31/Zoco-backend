using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ZocoApp.Data;
using ZocoApp.DTOs;
using ZocoApp.Models;

namespace ZocoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) => _context = context;

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListDto>>> GetAll()
        {
            var data = await _context.Users
                .Select(u => new UserListDto { Id = u.Id, Email = u.Email, Role = u.Role })
                .ToListAsync();

            return Ok(data);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDetailDto>> GetById(int id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u is null) return NotFound();

            var dto = new UserDetailDto { Id = u.Id, Email = u.Email, Role = u.Role };
            return Ok(dto);
        }

        
        [HttpPost]
        public async Task<ActionResult<UserDetailDto>> Create(CreateUserDto dto)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (exists) return BadRequest("El email ya está registrado.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = Hash(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var outDto = new UserDetailDto { Id = user.Id, Email = user.Email, Role = user.Role };
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, outDto);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            var u = await _context.Users.FindAsync(id);
            if (u is null) return NotFound();

            
            if (!u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _context.Users.AnyAsync(x => x.Email == dto.Email && x.Id != id);
                if (taken) return BadRequest("Ya existe un usuario con ese email.");
            }

            u.Email = dto.Email;
            u.Role = dto.Role;
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                u.PasswordHash = Hash(dto.NewPassword);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u is null) return NotFound();

            _context.Users.Remove(u);
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

