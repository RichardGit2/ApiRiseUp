using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpportunitiesController : ControllerBase
{
    private readonly ILogger<OpportunitiesController> _logger;
    private readonly DatabaseContext _context;

    public OpportunitiesController(ILogger<OpportunitiesController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<VolunteerResponse>> GetOpportunities(
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
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Opportunities.CountAsync();

            var response = new VolunteerResponse
            {
                Count = totalCount,
                Next = totalCount > skip + pageSize ? $"/api/opportunities?page={pageNumber + 1}" : null,
                Previous = pageNumber > 1 ? $"/api/opportunities?page={pageNumber - 1}" : null,
                Results = opportunities.Select(o => new OpportunityResponse
                {
                    Id = o.Id,
                    Url = o.Url,
                    Title = o.Title,
                    Description = o.Description,
                    RemoteOrOnline = o.RemoteOrOnline,
                    Organization = new OrganizationResponse
                    {
                        Name = o.Organization.Name,
                        Logo = o.Organization.Logo,
                        Url = o.Organization.Url
                    },
                    Activities = o.Activities.Select(a => new ActivityResponse
                    {
                        Name = a.Name,
                        Category = a.Category
                    }).ToList(),
                    Dates = o.Dates,
                    Duration = o.Duration,
                    Audience = new AudienceResponse
                    {
                        Scope = o.Scope,
                        Regions = o.Regions
                    }
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar oportunidades");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Opportunity>> GetOpportunity(int id)
    {
        var opportunity = await _context.Opportunities
            .Include(o => o.Organization)
            .Include(o => o.Activities)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (opportunity == null)
        {
            return NotFound();
        }

        return opportunity;
    }

    [Authorize(Roles = "Organization")]
    [HttpPost]
    public async Task<ActionResult<Opportunity>> CreateOpportunity([FromBody] CreateOpportunityRequest request)
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var opportunity = new Opportunity
        {
            Url = request.Url,
            Title = request.Title,
            Description = request.Description,
            RemoteOrOnline = request.RemoteOrOnline,
            OrganizationId = int.Parse(orgId),
            Dates = request.Dates,
            Duration = request.Duration,
            Scope = request.Scope,
            Regions = request.Regions,
            Activities = new List<Activity>()
        };

        _context.Opportunities.Add(opportunity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOpportunity), new { id = opportunity.Id }, opportunity);
    }

    [Authorize(Roles = "Organization")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOpportunity(int id, [FromBody] Opportunity updatedOpportunity)
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var opportunity = await _context.Opportunities.FindAsync(id);
        if (opportunity == null || opportunity.OrganizationId != int.Parse(orgId))
        {
            return NotFound();
        }

        // Atualizar campos
        opportunity.Title = updatedOpportunity.Title;
        opportunity.Description = updatedOpportunity.Description;
        opportunity.RemoteOrOnline = updatedOpportunity.RemoteOrOnline;
        opportunity.Dates = updatedOpportunity.Dates;
        opportunity.Duration = updatedOpportunity.Duration;
        opportunity.Scope = updatedOpportunity.Scope;
        opportunity.Regions = updatedOpportunity.Regions;

        // Atualizar atividades
        _context.Activities.RemoveRange(opportunity.Activities);
        opportunity.Activities = updatedOpportunity.Activities;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OpportunityExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [Authorize(Roles = "Organization")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOpportunity(int id)
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var opportunity = await _context.Opportunities.FindAsync(id);
        if (opportunity == null || opportunity.OrganizationId != int.Parse(orgId))
        {
            return NotFound();
        }

        _context.Opportunities.Remove(opportunity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OpportunityExists(int id)
    {
        return _context.Opportunities.Any(e => e.Id == id);
    }
}

public class CreateOpportunityRequest
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool RemoteOrOnline { get; set; }
    public string Dates { get; set; }
    public string Duration { get; set; }
    public string Scope { get; set; }
    public List<string> Regions { get; set; }
}

public class VolunteerResponse
{
    public int Count { get; set; }
    public string Next { get; set; }
    public string Previous { get; set; }
    public List<OpportunityResponse> Results { get; set; }
}

public class OpportunityResponse
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool RemoteOrOnline { get; set; }
    public OrganizationResponse Organization { get; set; }
    public List<ActivityResponse> Activities { get; set; }
    public string Dates { get; set; }
    public string Duration { get; set; }
    public AudienceResponse Audience { get; set; }
}

public class OrganizationResponse
{
    public string Name { get; set; }
    public string Logo { get; set; }
    public string Url { get; set; }
}

public class ActivityResponse
{
    public string Name { get; set; }
    public string Category { get; set; }
}

public class AudienceResponse
{
    public string Scope { get; set; }
    public List<string> Regions { get; set; }
} 