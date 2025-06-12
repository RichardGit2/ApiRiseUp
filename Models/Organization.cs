using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class Organization
{
    public int Id { get; set; }

    [Required]
    [StringLength(14)]
    public string CNPJ { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string Logo { get; set; } = string.Empty;

    [StringLength(200)]
    public string Website { get; set; } = string.Empty;

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(8)]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(2)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Organization";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
} 