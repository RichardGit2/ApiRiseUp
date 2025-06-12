using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUpAPI.Models
{
    public class Region
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int AudienceId { get; set; }

        [ForeignKey("AudienceId")]
        public Audience Audience { get; set; } = null!;
    }
} 