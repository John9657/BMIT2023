using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class FeeStructure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FeeType { get; set; } // Tuition, Lab, Library, Activity, Sports, etc.

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Currency { get; set; } = "USD";

        [Required]
        [StringLength(50)]
        public string ApplicableTo { get; set; } = "All"; // All, SpecificGroup, ByProgram

        [StringLength(100)]
        public string ApplicableValue { get; set; } = ""; // e.g., program name if ApplicableTo=ByProgram

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsMandatory { get; set; } = true;

        public DateTime EffectiveFrom { get; set; } = DateTime.Now;

        public DateTime? EffectiveTo { get; set; }

        public int? FrequencyInMonths { get; set; } // Null = One-time, 1 = Monthly, 12 = Yearly

        public bool IsRefundable { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
