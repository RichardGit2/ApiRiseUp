using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class Audience
{
    public int Id { get; set; }
    
    [Required]
    public string Scope { get; set; }
    
    public List<string> Regions { get; set; } = new();
    
    public int OpportunityId { get; set; }
    public Opportunity? Opportunity { get; set; }
} 