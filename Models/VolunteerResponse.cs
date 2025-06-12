namespace RiseUpAPI.Models;

public class VolunteerResponse
{
    public int Count { get; set; }
    public string Next { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public List<Opportunity> Results { get; set; } = new();
} 