using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class Audience
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Scope { get; set; }
    
    public string OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; }
    
    public List<Region> Regions { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 