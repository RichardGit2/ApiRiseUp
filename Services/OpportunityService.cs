using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly ApplicationDbContext _context;

        public OpportunityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Opportunity> opportunities, int totalItems)> SearchOpportunities(
            string search,
            string location,
            string type,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Opportunities
                .Include(o => o.Organization)
                .Include(o => o.Activities)
                .Include(o => o.Audience)
                    .ThenInclude(a => a.Regions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => 
                    o.Title.Contains(search) || 
                    o.Description.Contains(search) ||
                    o.Company.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(o => o.Location.Contains(location));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(o => o.Type == type);
            }

            var totalItems = await query.CountAsync();
            var opportunities = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (opportunities, totalItems);
        }

        public async Task<Opportunity> GetOpportunityById(string id)
        {
            return await _context.Opportunities
                .Include(o => o.Organization)
                .Include(o => o.Activities)
                .Include(o => o.Audience)
                    .ThenInclude(a => a.Regions)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Opportunity> CreateOpportunity(Opportunity opportunity)
        {
            opportunity.CreatedAt = DateTime.UtcNow;
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            return opportunity;
        }

        public async Task<Opportunity> UpdateOpportunity(string id, Opportunity opportunity)
        {
            var existingOpportunity = await _context.Opportunities.FindAsync(id);
            if (existingOpportunity == null)
                return null;

            _context.Entry(existingOpportunity).CurrentValues.SetValues(opportunity);
            await _context.SaveChangesAsync();
            return existingOpportunity;
        }

        public async Task DeleteOpportunity(string id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();
            }
        }
    }
} 