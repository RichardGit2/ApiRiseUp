using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models;

public class Opportunity
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Requirements { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Benefits { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Salary { get; set; } = string.Empty;

    [StringLength(200)]
    public string Url { get; set; } = string.Empty;

    public bool RemoteOrOnline { get; set; }

    [Required]
    public int OrganizationId { get; set; }

    [ForeignKey("OrganizationId")]
    public Organization Organization { get; set; } = null!;

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    [Required]
    [StringLength(100)]
    public string Dates { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Duration { get; set; } = string.Empty;

    public virtual Audience Audience { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}