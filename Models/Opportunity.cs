using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models;

public class Opportunity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Url { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public bool RemoteOrOnline { get; set; }

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; }

    public List<Activity> Activities { get; set; }

    [Required]
    public string Dates { get; set; }

    [Required]
    public string Duration { get; set; }

    [Required]
    public string Scope { get; set; }

    [Required]
    public List<string> Regions { get; set; }
} 