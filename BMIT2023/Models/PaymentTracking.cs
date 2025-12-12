using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class PaymentTracking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Initiated, Authorized, Captured, Settled, Failed, Cancelled

        [Required]
        public DateTime StatusChangedDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Details { get; set; } = ""; // Additional tracking info

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountTracked { get; set; }

        // Reconciliation info
        [StringLength(100)]
        public string BankReferenceNumber { get; set; } = "";

        public DateTime? BankClearingDate { get; set; }

        public bool IsMatched { get; set; } = false;

        public DateTime? MatchedDate { get; set; }

        [StringLength(500)]
        public string MatchingNotes { get; set; } = "";
    }
}
