using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class Refund
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RefundReference { get; set; } // REF-YYYY-XXXXX

        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        [ForeignKey("StudentBillingProfile")]
        public int StudentBillingProfileId { get; set; }
        public virtual StudentBillingProfile StudentBillingProfile { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }

        [Required]
        [StringLength(500)]
        public string RefundReason { get; set; } // Overpayment, Cancellation, Scholarship, Error, etc.

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Processing, Completed, Rejected, Cancelled

        [StringLength(50)]
        public string RefundMethod { get; set; } = "Original"; // Original, BankTransfer, Credit, Manual

        [StringLength(100)]
        public string BankAccountNumber { get; set; } = "";

        [StringLength(50)]
        public string BankCode { get; set; } = "";

        public DateTime RequestedDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string ApprovalNotes { get; set; } = "";

        [StringLength(500)]
        public string RejectionReason { get; set; } = "";

        public int? ApprovedByAdminId { get; set; } // Admin who approved the refund
    }
}
