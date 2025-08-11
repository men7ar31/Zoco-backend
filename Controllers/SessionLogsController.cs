using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZocoApp.Data;
using ZocoApp.Models;

namespace ZocoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionLogsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SessionLogsController(AppDbContext context) => _context = context;

        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<SessionLog>>> Mine()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _context.SessionLogs
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SessionLog>>> All()
        {
            return await _context.SessionLogs
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }
    }
}
