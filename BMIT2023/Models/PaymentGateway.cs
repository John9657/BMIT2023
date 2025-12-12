using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class PaymentGateway
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string GatewayName { get; set; } // Stripe, PayPal, Square, etc.

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsDefault { get; set; } = false;

        // API Credentials (encrypted in production)
        [StringLength(500)]
        public string ApiKey { get; set; } = "";

        [StringLength(500)]
        public string SecretKey { get; set; } = "";

        [StringLength(500)]
        public string ApiEndpoint { get; set; } = "";

        // Fees and Charges
        [Column(TypeName = "decimal(5,2)")]
        public decimal TransactionFeePercent { get; set; } = 0; // e.g., 2.9%

        [Column(TypeName = "decimal(18,2)")]
        public decimal TransactionFeeFixed { get; set; } = 0; // e.g., $0.30

        // Supported currencies
        [StringLength(500)]
        public string SupportedCurrencies { get; set; } = "USD,EUR,GBP"; // Comma-separated

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumAmount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumAmount { get; set; } = 999999.99m;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
