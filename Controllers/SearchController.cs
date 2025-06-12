using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly ApplicationDbContext _context;

    public SearchController(ILogger<SearchController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<VolunteerResponse>> SearchOpportunities(
        [FromQuery] string format = "json",
        [FromQuery] string page = "1")
    {
        try
        {
            var pageSize = 10;
            var pageNumber = int.Parse(page);
            var skip = (pageNumber - 1) * pageSize;

            var opportunities = await _context.Opportunities
                .Include(o => o.Organization)
                .Include(o => o.Activities)
                .Include(o => o.Audience)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Opportunities.CountAsync();

            var response = new VolunteerResponse
            {
                Count = totalCount,
                Next = totalCount > skip + pageSize ? $"/api/search?page={pageNumber + 1}" : null,
                Previous = pageNumber > 1 ? $"/api/search?page={pageNumber - 1}" : null,
                Results = opportunities
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar oportunidades");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 