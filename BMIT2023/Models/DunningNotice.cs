using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMIT2023.Models
{
    public class DunningNotice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NoticeNumber { get; set; } // DN-YYYY-XXXXX

        [ForeignKey("StudentBillingProfile")]
        public int StudentBillingProfileId { get; set; }
        public virtual StudentBillingProfile StudentBillingProfile { get; set; }

        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        [Required]
        [StringLength(50)]
        public string NoticeLevel { get; set; } = "Level1"; // Level1 (Reminder), Level2 (Warning), Level3 (Escalation), Level4 (Final)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OutstandingAmount { get; set; }

        [Required]
        public DateTime DueDateOriginal { get; set; }

        [Required]
        public DateTime DaysOverdue { get; set; }

        [Required]
        public DateTime NoticeDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime NoticeTargetDate { get; set; } // When the student should pay by

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Sent"; // Sent, Read, Acknowledged, Resolved, Escalated

        [StringLength(500)]
        public string NoticeMessage { get; set; } = "";

        [StringLength(500)]
        public string NoticeTemplate { get; set; } = ""; // Which template was used

        public bool IsAutomatic { get; set; } = true; // Auto-generated or manual

        public DateTime? ResolutionDate { get; set; }

        [StringLength(500)]
        public string ResolutionNotes { get; set; } = "";

        // Contact info
        [StringLength(100)]
        public string NotificationMethod { get; set; } = "Email"; // Email, SMS, Phone, Mail

        public bool WasAcknowledged { get; set; } = false;

        public DateTime? AcknowledgedDate { get; set; }

        // Late fees
        [Column(TypeName = "decimal(18,2)")]
        public decimal LateFeeCharged { get; set; } = 0;

        public DateTime? LateFeeAppliedDate { get; set; }

        // Escalation
        public int? EscalationLevel { get; set; } = 0;

        public DateTime? EscalatedToAdminDate { get; set; }
    }
}
