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
    private readonly ApplicationDbContext _context;

    public OpportunitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Opportunity>>> GetOpportunities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? location = null,
        [FromQuery] string? type = null)
    {
        var query = _context.Opportunities
            .Include(o => o.Organization)
            .Include(o => o.Activities)
            .Include(o => o.Audience)
                .ThenInclude(a => a.Regions)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(o => o.Title.Contains(search) || o.Description.Contains(search));
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(o => o.Location.Contains(location));
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(o => o.Type == type);
        }

        var totalCount = await query.CountAsync();
        var opportunities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new
        {
            count = totalCount,
            next = page * pageSize < totalCount ? $"/api/opportunities?page={page + 1}&pageSize={pageSize}" : null,
            previous = page > 1 ? $"/api/opportunities?page={page - 1}&pageSize={pageSize}" : null,
            results = opportunities
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Opportunity>> GetOpportunity(int id)
    {
        var opportunity = await _context.Opportunities
            .Include(o => o.Organization)
            .Include(o => o.Activities)
            .Include(o => o.Audience)
                .ThenInclude(a => a.Regions)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (opportunity == null)
        {
            return NotFound();
        }

        return opportunity;
    }

    [Authorize(Roles = "Organization")]
    [HttpPost]
    public async Task<ActionResult<Opportunity>> CreateOpportunity([FromBody] Opportunity opportunity)
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var organization = await _context.Organizations.FindAsync(int.Parse(orgId));
        if (organization == null)
        {
            return NotFound("Organização não encontrada");
        }

        opportunity.OrganizationId = organization.Id;
        opportunity.CreatedAt = DateTime.UtcNow;

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
        if (opportunity == null)
        {
            return NotFound();
        }

        if (opportunity.OrganizationId != int.Parse(orgId))
        {
            return Forbid();
        }

        // Atualizar apenas campos permitidos
        opportunity.Title = updatedOpportunity.Title;
        opportunity.Description = updatedOpportunity.Description;
        opportunity.Company = updatedOpportunity.Company;
        opportunity.Location = updatedOpportunity.Location;
        opportunity.Type = updatedOpportunity.Type;
        opportunity.Requirements = updatedOpportunity.Requirements;
        opportunity.Benefits = updatedOpportunity.Benefits;
        opportunity.Salary = updatedOpportunity.Salary;
        opportunity.Url = updatedOpportunity.Url;
        opportunity.RemoteOrOnline = updatedOpportunity.RemoteOrOnline;
        opportunity.Dates = updatedOpportunity.Dates;
        opportunity.Duration = updatedOpportunity.Duration;

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
        if (opportunity == null)
        {
            return NotFound();
        }

        if (opportunity.OrganizationId != int.Parse(orgId))
        {
            return Forbid();
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