using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public OrganizationsController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Organization>> Register([FromBody] Organization organization)
    {
        if (await _context.Organizations.AnyAsync(o => o.Email == organization.Email))
        {
            return BadRequest("Email já está em uso");
        }

        if (await _context.Organizations.AnyAsync(o => o.CNPJ == organization.CNPJ))
        {
            return BadRequest("CNPJ já está em uso");
        }

        organization.CreatedAt = DateTime.UtcNow;
        organization.UpdatedAt = DateTime.UtcNow;

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfile), new { id = organization.Id }, organization);
    }

    [Authorize(Roles = "Organization")]
    [HttpGet("profile")]
    public async Task<ActionResult<Organization>> GetProfile()
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var organization = await _context.Organizations
            .Include(o => o.Opportunities)
            .FirstOrDefaultAsync(o => o.Id == int.Parse(orgId));

        if (organization == null)
        {
            return NotFound();
        }

        return organization;
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] Organization updatedOrg)
    {
        var orgId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (orgId == null)
        {
            return Unauthorized();
        }

        var organization = await _context.Organizations.FindAsync(int.Parse(orgId));
        if (organization == null)
        {
            return NotFound();
        }

        // Atualizar apenas campos permitidos
        organization.Name = updatedOrg.Name;
        organization.Logo = updatedOrg.Logo;
        organization.ZipCode = updatedOrg.ZipCode;
        organization.Street = updatedOrg.Street;
        organization.City = updatedOrg.City;
        organization.State = updatedOrg.State;
        organization.Country = updatedOrg.Country;
        organization.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrganizationExists(organization.Id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    private bool OrganizationExists(int id)
    {
        return _context.Organizations.Any(e => e.Id == id);
    }
} 