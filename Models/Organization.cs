using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class Organization
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public string Logo { get; set; }

    [Required]
    [MaxLength(200)]
    public string Url { get; set; }

    [Required]
    [MaxLength(14)]
    public string CNPJ { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string Password { get; set; }

    [Required]
    [MaxLength(100)]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [Required]
    [MaxLength(2)]
    public string State { get; set; }

    [Required]
    [MaxLength(8)]
    public string ZipCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string Country { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
} 