namespace RiseUpAPI.Models;

public class VolunteerResponse<T>
{
    public int Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public List<T> Results { get; set; } = new();
}

public class VolunteerResponse : VolunteerResponse<Opportunity>
{
}