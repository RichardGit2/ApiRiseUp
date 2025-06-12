using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(11)]
    public string CPF { get; set; } = "string";

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = "string";

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = "user@example.com";

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = "string";

    [Required]
    [StringLength(8)]
    public string ZipCode { get; set; } = "string";

    [Required]
    [StringLength(100)]
    public string Street { get; set; } = "string";

    [Required]
    [StringLength(50)]
    public string City { get; set; } = "string";

    [Required]
    [StringLength(2)]
    public string State { get; set; } = "st";

    [Required]
    [StringLength(50)]
    public string Country { get; set; } = "string";

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "string";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 