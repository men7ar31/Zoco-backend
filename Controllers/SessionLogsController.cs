using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZocoApp.Data;
using ZocoApp.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SessionLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SessionLogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
public async Task<ActionResult<IEnumerable<SessionLog>>> GetSessionLogs()
{
    return await _context.SessionLogs
        .OrderByDescending(s => s.FechaInicio)
        .ToListAsync();
}
    
}
