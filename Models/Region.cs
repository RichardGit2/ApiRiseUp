using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models
{
    public class Region
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string AudienceId { get; set; }
        public Audience Audience { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 