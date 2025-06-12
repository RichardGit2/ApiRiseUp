using RiseUpAPI.Models;

namespace RiseUpAPI.Services;

public interface IOpportunityService
{
    Task<(List<Opportunity> opportunities, int totalCount)> SearchOpportunities(
        string search, string location, string type, int page, int pageSize);
    
    Task<Opportunity?> GetOpportunity(int id);
}