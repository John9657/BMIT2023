using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class StudentBillingProfile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        [StringLength(50)]
        public string BillingStatus { get; set; } = "Active"; // Active, Inactive, OnHold, Suspended

        [Required]
        public decimal TotalOutstanding { get; set; } = 0;

        [Required]
        public decimal TotalPaid { get; set; } = 0;

        [Required]
        public decimal CreditBalance { get; set; } = 0;

        public string PaymentMethod { get; set; } = ""; // CreditCard, BankTransfer, Cheque, Cash

        public string BillingAddress { get; set; } = "";

        public string Phone { get; set; } = "";

        public bool AutoPaymentEnabled { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastPaymentDate { get; set; }

        public int? PrimaryContactId { get; set; } // Contact person ID for billing

        // Navigation properties
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    }
}
