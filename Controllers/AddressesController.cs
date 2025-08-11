using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZocoApp.Data;
using ZocoApp.DTOs;
using ZocoApp.Models;

namespace ZocoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AddressesController(AppDbContext context) => _context = context;

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin() => User.FindFirstValue(ClaimTypes.Role) == "Admin";


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> Get([FromQuery] int? userId)
        {
            IQueryable<Address> q = _context.Addresses.AsQueryable();

            if (IsAdmin())
            {
                if (userId.HasValue) q = q.Where(a => a.UserId == userId.Value);
                
            }
            else
            {
                var me = CurrentUserId();
                q = q.Where(a => a.UserId == me);
            }

            return await q.OrderBy(a => a.Id).ToListAsync();
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr is null) return NotFound();

            if (addr.UserId != CurrentUserId() && !IsAdmin()) return Forbid();
            return addr;
        }

        
        [HttpPost]
        public async Task<ActionResult<Address>> Create([FromBody] AddressDto dto, [FromQuery] int? userId)
        {
            var ownerId = IsAdmin() && userId.HasValue ? userId.Value : CurrentUserId();

            var addr = new Address
            {
                Street = dto.Street,
                City = dto.City,
                Country = dto.Country,
                UserId = ownerId
            };

            _context.Addresses.Add(addr);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = addr.Id }, addr);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddressDto dto)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr is null) return NotFound();

            if (addr.UserId != CurrentUserId() && !IsAdmin()) return Forbid();

            addr.Street = dto.Street;
            addr.City = dto.City;
            addr.Country = dto.Country;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr is null) return NotFound();

            if (addr.UserId != CurrentUserId() && !IsAdmin()) return Forbid();

            _context.Addresses.Remove(addr);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
