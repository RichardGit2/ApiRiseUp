using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class Organization
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; }

    public string Description { get; set; }

    public string Logo { get; set; }

    public string Website { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Opportunity> Opportunities { get; set; } = new();
} 