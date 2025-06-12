using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int OpportunityId { get; set; }

        [ForeignKey("OpportunityId")]
        public Opportunity Opportunity { get; set; } = null!;
    }
} 