using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Services;

public class OpportunityService : IOpportunityService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OpportunityService> _logger;

    public OpportunityService(ApplicationDbContext context, ILogger<OpportunityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(List<Opportunity> opportunities, int totalCount)> SearchOpportunities(
        string search, string location, string type, int page, int pageSize)
    {
        try
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

            var totalItems = await query.CountAsync();
            var opportunities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (opportunities, totalItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar oportunidades");
            return (new List<Opportunity>(), 0);
        }
    }

    public async Task<Opportunity?> GetOpportunity(int id)
    {
        try
        {
            return await _context.Opportunities
                .Include(o => o.Organization)
                .Include(o => o.Activities)
                .Include(o => o.Audience)
                .ThenInclude(a => a.Regions)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar oportunidade com ID {id}");
            return null;
        }
    }

    public async Task<Opportunity> CreateOpportunity(Opportunity opportunity)
    {
        opportunity.CreatedAt = DateTime.UtcNow;
        _context.Opportunities.Add(opportunity);
        await _context.SaveChangesAsync();
        return opportunity;
    }

    public async Task<Opportunity> UpdateOpportunity(int id, Opportunity opportunity)
    {
        var existingOpportunity = await _context.Opportunities.FindAsync(id);
        if (existingOpportunity == null)
            return null;

        _context.Entry(existingOpportunity).CurrentValues.SetValues(opportunity);
        await _context.SaveChangesAsync();
        return existingOpportunity;
    }

    public async Task DeleteOpportunity(int id)
    {
        var opportunity = await _context.Opportunities.FindAsync(id);
        if (opportunity != null)
        {
            _context.Opportunities.Remove(opportunity);
            await _context.SaveChangesAsync();
        }
    }
}