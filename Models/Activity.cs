using System.ComponentModel.DataAnnotations;

namespace RiseUpAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }
        
        public int OpportunityId { get; set; }
        public Opportunity Opportunity { get; set; }
    }
} 