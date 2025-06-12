using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models;

public class Audience
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Scope { get; set; } = string.Empty;

    public ICollection<Region> Regions { get; set; } = new List<Region>();

    public int OpportunityId { get; set; }

    [ForeignKey("OpportunityId")]
    public Opportunity Opportunity { get; set; } = null!;
} 