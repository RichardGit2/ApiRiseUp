using Microsoft.AspNetCore.Identity;

namespace RiseUpAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
}