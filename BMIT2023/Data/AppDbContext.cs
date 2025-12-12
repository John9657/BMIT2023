using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BMIT2023.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        // Payment and Billing DbSets
        public DbSet<StudentBillingProfile> StudentBillingProfiles { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentGateway> PaymentGateways { get; set; }
        public DbSet<PaymentTracking> PaymentTrackings { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<DunningNotice> DunningNotices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- SEED DATA for Admin and Teacher ---

            // Admin Data
            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 1, AdminId = "A001", FullName = "System Admin", Email = "admin@bmit.com", Password = "password123" }
            );

            // Teacher Data
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, TeacherId = "T001", FullName = "Dr. Alice Smith", Email = "alice.smith@bmit.edu", Password = "teacherpass" },
                new Teacher { Id = 2, TeacherId = "T002", FullName = "Mr. Bob Johnson", Email = "bob.j@bmit.edu", Password = "teacherpass" }
            );

            // Fee Structure Seed Data
            modelBuilder.Entity<FeeStructure>().HasData(
                new FeeStructure { Id = 1, FeeType = "Tuition", Description = "Monthly tuition fee", Amount = 5000, Currency = "USD", ApplicableTo = "All", IsActive = true, IsMandatory = true, FrequencyInMonths = 1, IsRefundable = true },
                new FeeStructure { Id = 2, FeeType = "Lab Fee", Description = "Laboratory access and materials", Amount = 500, Currency = "USD", ApplicableTo = "All", IsActive = true, IsMandatory = false, FrequencyInMonths = null, IsRefundable = false },
                new FeeStructure { Id = 3, FeeType = "Library Fee", Description = "Library membership and resources", Amount = 200, Currency = "USD", ApplicableTo = "All", IsActive = true, IsMandatory = true, FrequencyInMonths = 12, IsRefundable = false },
                new FeeStructure { Id = 4, FeeType = "Activity Fee", Description = "Student activities and events", Amount = 300, Currency = "USD", ApplicableTo = "All", IsActive = true, IsMandatory = false, FrequencyInMonths = 12, IsRefundable = true },
                new FeeStructure { Id = 5, FeeType = "Sports Fee", Description = "Sports facilities and programs", Amount = 400, Currency = "USD", ApplicableTo = "All", IsActive = true, IsMandatory = false, FrequencyInMonths = 12, IsRefundable = true }
            );

            // Payment Gateway Seed Data
            modelBuilder.Entity<PaymentGateway>().HasData(
                new PaymentGateway { Id = 1, GatewayName = "Stripe", Description = "Stripe payment gateway for credit/debit cards", IsActive = true, IsDefault = true, TransactionFeePercent = 2.9M, TransactionFeeFixed = 0.30M, SupportedCurrencies = "USD,EUR,GBP,CAD", MinimumAmount = 0.50M, MaximumAmount = 999999.99M },
                new PaymentGateway { Id = 2, GatewayName = "PayPal", Description = "PayPal payment solution", IsActive = true, IsDefault = false, TransactionFeePercent = 3.49M, TransactionFeeFixed = 0.49M, SupportedCurrencies = "USD,EUR,GBP", MinimumAmount = 1, MaximumAmount = 999999.99M },
                new PaymentGateway { Id = 3, GatewayName = "Bank Transfer", Description = "Direct bank transfer", IsActive = true, IsDefault = false, TransactionFeePercent = 0, TransactionFeeFixed = 5, SupportedCurrencies = "USD", MinimumAmount = 100, MaximumAmount = 999999.99M },
                new PaymentGateway { Id = 4, GatewayName = "Cash / Check", Description = "Cash or check payment", IsActive = true, IsDefault = false, TransactionFeePercent = 0, TransactionFeeFixed = 0, SupportedCurrencies = "USD", MinimumAmount = 0, MaximumAmount = 999999.99M }
            );

            modelBuilder.Entity<Student>().HasIndex(s => s.StudentId).IsUnique();
            modelBuilder.Entity<Teacher>().HasIndex(t => t.TeacherId).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(a => a.AdminId).IsUnique();
        }
    }
}