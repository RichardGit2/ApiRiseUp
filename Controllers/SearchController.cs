using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;
using RiseUpAPI.Services;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IOpportunityService _opportunityService;

    public SearchController(ILogger<SearchController> logger, ApplicationDbContext context, IOpportunityService opportunityService)
    {
        _logger = logger;
        _context = context;
        _opportunityService = opportunityService;
    }

    [HttpGet]
    public async Task<ActionResult<VolunteerResponse>> Search(
        [FromQuery] string format = "json",
        [FromQuery] string page = "1",
        [FromQuery] string search = "",
        [FromQuery] string location = "",
        [FromQuery] string type = "")
    {
        try
        {
            var pageNumber = int.Parse(page);
            var pageSize = 10;

            var (opportunities, totalItems) = await _opportunityService.SearchOpportunities(
                search, location, type, pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var hasNextPage = pageNumber < totalPages;

            var response = new VolunteerResponse
            {
                Count = totalItems,
                Next = hasNextPage ? $"/api/search?page={pageNumber + 1}" : null,
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