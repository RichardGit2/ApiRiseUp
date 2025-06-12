using RiseUpAPI.Models;

namespace RiseUpAPI.Services
{
    public interface IOpportunityService
    {
        Task<(List<Opportunity> opportunities, int totalItems)> SearchOpportunities(
            string search,
            string location,
            string type,
            int pageNumber,
            int pageSize);

        Task<Opportunity> GetOpportunityById(string id);
        Task<Opportunity> CreateOpportunity(Opportunity opportunity);
        Task<Opportunity> UpdateOpportunity(string id, Opportunity opportunity);
        Task DeleteOpportunity(string id);
    }
} 