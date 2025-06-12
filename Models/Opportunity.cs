using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models;

public class Opportunity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(200)]
    public string Url { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public bool RemoteOrOnline { get; set; }

    [Required]
    public string OrganizationId { get; set; }
    public Organization Organization { get; set; }

    public List<Activity> Activities { get; set; } = new();

    [Required]
    public string Dates { get; set; }

    [Required]
    public string Duration { get; set; }

    [Required]
    public Audience Audience { get; set; }

    [Required]
    public string Scope { get; set; }

    [Required]
    public List<string> Regions { get; set; }

    public string Company { get; set; }
    public string Location { get; set; }
    public string Type { get; set; }
    public string Requirements { get; set; }
    public string Benefits { get; set; }
    public string Salary { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 