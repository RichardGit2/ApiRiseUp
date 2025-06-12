using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Category { get; set; }
        
        public int OpportunityId { get; set; }
        public Opportunity? Opportunity { get; set; }
    }
} 