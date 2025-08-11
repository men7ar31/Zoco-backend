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
    public class StudiesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StudiesController(AppDbContext context) => _context = context;

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin() => User.FindFirstValue(ClaimTypes.Role) == "Admin";

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudyDetailDto>>> Get([FromQuery] int? userId)
        {
            IQueryable<Study> q = _context.Studies.AsQueryable();

            if (IsAdmin())
            {
                if (userId.HasValue) q = q.Where(s => s.UserId == userId.Value);
            }
            else
            {
                q = q.Where(s => s.UserId == CurrentUserId());
            }

            var list = await q
                .OrderBy(s => s.Id)
                .Select(s => new StudyDetailDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Institution = s.Institution,
                    CompletedAt = s.CompletedAt,
                    UserId = s.UserId
                })
                .ToListAsync();

            return Ok(list);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudyDetailDto>> GetStudy(int id)
        {
            var s = await _context.Studies.FindAsync(id);
            if (s is null) return NotFound();
            if (s.UserId != CurrentUserId() && !IsAdmin()) return Forbid();

            return new StudyDetailDto
            {
                Id = s.Id,
                Title = s.Title,
                Institution = s.Institution,
                CompletedAt = s.CompletedAt,
                UserId = s.UserId
            };
        }

        
        [HttpPost]
        public async Task<ActionResult<StudyDetailDto>> Create([FromBody] StudyDto dto, [FromQuery] int? userId)
        {
            var ownerId = IsAdmin() && userId.HasValue ? userId.Value : CurrentUserId();

            var study = new Study
            {
                Title = dto.Title,
                Institution = dto.Institution,
                CompletedAt = dto.CompletedAt,
                UserId = ownerId
            };

            _context.Studies.Add(study);
            await _context.SaveChangesAsync();

            var outDto = new StudyDetailDto
            {
                Id = study.Id,
                Title = study.Title,
                Institution = study.Institution,
                CompletedAt = study.CompletedAt,
                UserId = study.UserId
            };
            return CreatedAtAction(nameof(GetStudy), new { id = study.Id }, outDto);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudyDto dto)
        {
            var s = await _context.Studies.FindAsync(id);
            if (s is null) return NotFound();
            if (s.UserId != CurrentUserId() && !IsAdmin()) return Forbid();

            s.Title = dto.Title;
            s.Institution = dto.Institution;
            s.CompletedAt = dto.CompletedAt;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Studies.FindAsync(id);
            if (s is null) return NotFound();
            if (s.UserId != CurrentUserId() && !IsAdmin()) return Forbid();

            _context.Studies.Remove(s);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

