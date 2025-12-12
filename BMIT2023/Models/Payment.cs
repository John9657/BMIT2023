using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentReference { get; set; } // PAY-YYYY-XXXXX

        [ForeignKey("StudentBillingProfile")]
        public int StudentBillingProfileId { get; set; }
        public virtual StudentBillingProfile StudentBillingProfile { get; set; }

        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, BankTransfer, Cheque, Cash, OnlineGateway

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Successful, Failed, Cancelled, Refunded

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public DateTime? ProcessedDate { get; set; }

        [StringLength(100)]
        public string TransactionId { get; set; } = ""; // Gateway transaction ID

        [StringLength(500)]
        public string Notes { get; set; } = "";

        public bool IsReconciled { get; set; } = false;

        public DateTime? ReconciledDate { get; set; }

        // Payment Gateway Integration
        [ForeignKey("PaymentGateway")]
        public int? PaymentGatewayId { get; set; }
        public virtual PaymentGateway PaymentGateway { get; set; }

        [StringLength(100)]
        public string GatewayResponse { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal GatewayFeeAmount { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<PaymentTracking> PaymentTrackings { get; set; } = new List<PaymentTracking>();
    }
}
