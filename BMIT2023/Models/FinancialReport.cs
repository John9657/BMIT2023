using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class FinancialReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ReportName { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportType { get; set; } // DailyRevenue, MonthlyRevenue, AnnualRevenue, OutstandingInvoices, StudentAging, FeeCollection, etc.

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvoiced { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCollected { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOutstanding { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRefunded { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCredits { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CollectionRate { get; set; } = 0; // Percentage

        [StringLength(500)]
        public string Summary { get; set; } = "";

        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        public int? GeneratedByAdminId { get; set; }

        // Additional metrics
        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageInvoiceAmount { get; set; } = 0;

        public int TotalInvoiceCount { get; set; } = 0;

        public int TotalPaidInvoiceCount { get; set; } = 0;

        public int TotalOverdueInvoiceCount { get; set; } = 0;

        [StringLength(500)]
        public string JsonData { get; set; } = ""; // Store detailed data as JSON for flexibility
    }
}
