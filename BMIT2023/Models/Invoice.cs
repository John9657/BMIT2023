using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } // Auto-generated: INV-YYYY-XXXXX

        [ForeignKey("StudentBillingProfile")]
        public int StudentBillingProfileId { get; set; }
        public virtual StudentBillingProfile StudentBillingProfile { get; set; }

        [ForeignKey("FeeStructure")]
        public int FeeStructureId { get; set; }
        public virtual FeeStructure FeeStructure { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Issued, PartiallyPaid, Paid, Overdue, Cancelled

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountDue { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = "";

        public bool IsRecurring { get; set; } = false;

        public DateTime? RecurrenceEndDate { get; set; }

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    }
}
