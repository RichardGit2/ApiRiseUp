using Microsoft.AspNetCore.Identity;

namespace RiseUpAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string Organization { get; set; }
} 