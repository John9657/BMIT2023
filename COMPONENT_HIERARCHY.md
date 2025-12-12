# Payment & Billing System - Component Hierarchy & API Reference

## 🏛️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    ASP.NET Core MVC Application               │
├─────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────┐    │
│  │            Views (Razor Templates)                    │    │
│  │  Dashboard │ Invoices │ Payments │ Refunds │ Reports │    │
│  │  Fees │ Dunning │ PaymentGateways                    │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────┐    │
│  │         BillingController (50+ Actions)              │    │
│  │  Dashboard │ Invoice │ Payment │ Refund │ Report    │    │
│  │  Fee │ Dunning │ Gateway Management                  │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────┐    │
│  │              Service Layer (7 Services)              │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ StudentBillingProfileService (8 methods)   │    │    │
│  │  │  • GetProfileByIdAsync                      │    │    │
│  │  │  • GetProfileByStudentIdAsync               │    │    │
│  │  │  • CreateProfileAsync                       │    │    │
│  │  │  • UpdateProfileAsync                       │    │    │
│  │  │  • GetAllProfilesAsync                      │    │    │
│  │  │  • UpdateProfileStatusAsync                 │    │    │
│  │  │  • GetTotalOutstandingAsync                 │    │    │
│  │  │  • GetTotalPaidAsync                        │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ FeeStructureService (8 methods)            │    │    │
│  │  │  • GetAllFeesAsync                          │    │    │
│  │  │  • GetFeeByIdAsync                          │    │    │
│  │  │  • GetActiveFeesAsync                       │    │    │
│  │  │  • GetMandatoryFeesAsync                    │    │    │
│  │  │  • CreateFeeAsync                           │    │    │
│  │  │  • UpdateFeeAsync                           │    │    │
│  │  │  • DeleteFeeAsync                           │    │    │
│  │  │  • CalculateTotalFeesForStudentAsync        │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ InvoiceService (11 methods)                │    │    │
│  │  │  • GetAllInvoicesAsync                      │    │    │
│  │  │  • GetInvoiceByIdAsync                      │    │    │
│  │  │  • GetInvoicesByStudentAsync                │    │    │
│  │  │  • GetOverdueInvoicesAsync                  │    │    │
│  │  │  • GetPendingInvoicesAsync                  │    │    │
│  │  │  • CreateInvoiceAsync                       │    │    │
│  │  │  • UpdateInvoiceAsync                       │    │    │
│  │  │  • GenerateInvoiceNumberAsync               │    │    │
│  │  │  • GetInvoicesByDateRangeAsync              │    │    │
│  │  │  • UpdateInvoiceStatusAsync                 │    │    │
│  │  │  • (internal) UpdateBillingProfileAsync     │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ PaymentService (12 methods)                │    │    │
│  │  │  • GetAllPaymentsAsync                      │    │    │
│  │  │  • GetPaymentByIdAsync                      │    │    │
│  │  │  • GetPaymentsByStudentAsync                │    │    │
│  │  │  • GetPaymentsByInvoiceAsync                │    │    │
│  │  │  • GetPaymentsByDateRangeAsync              │    │    │
│  │  │  • GetPendingPaymentsAsync                  │    │    │
│  │  │  • CreatePaymentAsync                       │    │    │
│  │  │  • UpdatePaymentAsync                       │    │    │
│  │  │  • GeneratePaymentReferenceAsync            │    │    │
│  │  │  • ProcessPaymentAsync                      │    │    │
│  │  │  • CalculateGatewayFeesAsync                │    │    │
│  │  │  • GetUnreconciledPaymentsAsync             │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ RefundService (10 methods)                 │    │    │
│  │  │  • GetAllRefundsAsync                       │    │    │
│  │  │  • GetRefundByIdAsync                       │    │    │
│  │  │  • GetRefundsByStudentAsync                 │    │    │
│  │  │  • GetPendingRefundsAsync                   │    │    │
│  │  │  • GetApprovedRefundsAsync                  │    │    │
│  │  │  • CreateRefundAsync                        │    │    │
│  │  │  • UpdateRefundAsync                        │    │    │
│  │  │  • ApproveRefundAsync                       │    │    │
│  │  │  • RejectRefundAsync                        │    │    │
│  │  │  • GenerateRefundReferenceAsync             │    │    │
│  │  │  • ProcessRefundAsync                       │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ FinancialReportService (13 methods)        │    │    │
│  │  │  • GenerateDailyRevenueReportAsync          │    │    │
│  │  │  • GenerateMonthlyRevenueReportAsync        │    │    │
│  │  │  • GenerateAnnualRevenueReportAsync         │    │    │
│  │  │  • GenerateOutstandingInvoicesReportAsync   │    │    │
│  │  │  • GenerateStudentAgingReportAsync          │    │    │
│  │  │  • GenerateFeeCollectionReportAsync         │    │    │
│  │  │  • GetAllReportsAsync                       │    │    │
│  │  │  • GetReportByIdAsync                       │    │    │
│  │  │  • GetReportsByTypeAsync                    │    │    │
│  │  │  • GetTotalInvoicedAmountAsync              │    │    │
│  │  │  • GetTotalCollectedAmountAsync             │    │    │
│  │  │  • (internal) CalculateReportMetrics        │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │ DunningService (12 methods)                │    │    │
│  │  │  • GetAllNoticesAsync                       │    │    │
│  │  │  • GetNoticeByIdAsync                       │    │    │
│  │  │  • GetNoticesByStudentAsync                 │    │    │
│  │  │  • GetPendingNoticesAsync                   │    │    │
│  │  │  • GetOverdueNoticesAsync                   │    │    │
│  │  │  • CreateDunningNoticeAsync                 │    │    │
│  │  │  • UpdateNoticeAsync                        │    │    │
│  │  │  • GenerateNoticeNumberAsync                │    │    │
│  │  │  • GenerateAutomaticDunningNoticesAsync     │    │    │
│  │  │  • MarkNoticeAsResolvedAsync                │    │    │
│  │  │  • EscalateNoticeAsync                      │    │    │
│  │  │  • GetNoticesByLevelAsync                   │    │    │
│  │  │  • (internal) GetNoticeLevel                │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │                                                       │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────┐    │
│  │         Entity Framework Core (AppDbContext)          │    │
│  │                                                       │    │
│  │  DbSet<StudentBillingProfile>                        │    │
│  │  DbSet<FeeStructure>                                 │    │
│  │  DbSet<Invoice>                                      │    │
│  │  DbSet<InvoiceLineItem>                              │    │
│  │  DbSet<Payment>                                      │    │
│  │  DbSet<PaymentGateway>                               │    │
│  │  DbSet<PaymentTracking>                              │    │
│  │  DbSet<Refund>                                       │    │
│  │  DbSet<FinancialReport>                              │    │
│  │  DbSet<DunningNotice>                                │    │
│  │                                                       │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────┐    │
│  │            SQL Server Database                        │    │
│  │   10 Tables │ 16 Foreign Keys │ 10 Indexes           │    │
│  └──────────────────────────────────────────────────────┘    │
│                                                                 │
└─────────────────────────────────────────────────────────────┘
```

## 📋 Complete Method Reference

### StudentBillingProfileService

```csharp
public interface IStudentBillingProfileService
{
    Task<StudentBillingProfile> GetProfileByIdAsync(int id);
    Task<StudentBillingProfile> GetProfileByStudentIdAsync(int studentId);
    Task<StudentBillingProfile> CreateProfileAsync(StudentBillingProfile profile);
    Task UpdateProfileAsync(StudentBillingProfile profile);
    Task<List<StudentBillingProfile>> GetAllProfilesAsync();
    Task<List<StudentBillingProfile>> GetProfilesByStatusAsync(string status);
    Task<decimal> GetTotalOutstandingAsync(int profileId);
    Task<decimal> GetTotalPaidAsync(int profileId);
    Task UpdateProfileStatusAsync(int profileId, string status);
}
```

### FeeStructureService

```csharp
public interface IFeeStructureService
{
    Task<List<FeeStructure>> GetAllFeesAsync();
    Task<FeeStructure> GetFeeByIdAsync(int id);
    Task<List<FeeStructure>> GetActiveFeesAsync();
    Task<List<FeeStructure>> GetMandatoryFeesAsync();
    Task CreateFeeAsync(FeeStructure fee);
    Task UpdateFeeAsync(FeeStructure fee);
    Task DeleteFeeAsync(int id);
    Task<decimal> CalculateTotalFeesForStudentAsync(int studentId);
}
```

### InvoiceService

```csharp
public interface IInvoiceService
{
    Task<List<Invoice>> GetAllInvoicesAsync();
    Task<Invoice> GetInvoiceByIdAsync(int id);
    Task<List<Invoice>> GetInvoicesByStudentAsync(int billingProfileId);
    Task<List<Invoice>> GetOverdueInvoicesAsync();
    Task<List<Invoice>> GetPendingInvoicesAsync();
    Task<Invoice> CreateInvoiceAsync(Invoice invoice);
    Task UpdateInvoiceAsync(Invoice invoice);
    Task<string> GenerateInvoiceNumberAsync();
    Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task UpdateInvoiceStatusAsync(int invoiceId, string status);
}
```

### PaymentService

```csharp
public interface IPaymentService
{
    Task<List<Payment>> GetAllPaymentsAsync();
    Task<Payment> GetPaymentByIdAsync(int id);
    Task<List<Payment>> GetPaymentsByStudentAsync(int billingProfileId);
    Task<List<Payment>> GetPaymentsByInvoiceAsync(int invoiceId);
    Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Payment>> GetPendingPaymentsAsync();
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task<string> GeneratePaymentReferenceAsync();
    Task ProcessPaymentAsync(int paymentId);
    Task<decimal> CalculateGatewayFeesAsync(decimal amount, int gatewayId);
    Task<List<Payment>> GetUnreconciledPaymentsAsync();
}
```

### RefundService

```csharp
public interface IRefundService
{
    Task<List<Refund>> GetAllRefundsAsync();
    Task<Refund> GetRefundByIdAsync(int id);
    Task<List<Refund>> GetRefundsByStudentAsync(int billingProfileId);
    Task<List<Refund>> GetPendingRefundsAsync();
    Task<List<Refund>> GetApprovedRefundsAsync();
    Task<Refund> CreateRefundAsync(Refund refund);
    Task UpdateRefundAsync(Refund refund);
    Task ApproveRefundAsync(int refundId, int adminId, string notes = "");
    Task RejectRefundAsync(int refundId, string reason);
    Task<string> GenerateRefundReferenceAsync();
    Task ProcessRefundAsync(int refundId);
}
```

### FinancialReportService

```csharp
public interface IFinancialReportService
{
    Task<FinancialReport> GenerateDailyRevenueReportAsync(DateTime date, int adminId);
    Task<FinancialReport> GenerateMonthlyRevenueReportAsync(int year, int month, int adminId);
    Task<FinancialReport> GenerateAnnualRevenueReportAsync(int year, int adminId);
    Task<FinancialReport> GenerateOutstandingInvoicesReportAsync(int adminId);
    Task<FinancialReport> GenerateStudentAgingReportAsync(int adminId);
    Task<FinancialReport> GenerateFeeCollectionReportAsync(DateTime startDate, DateTime endDate, int adminId);
    Task<List<FinancialReport>> GetAllReportsAsync();
    Task<FinancialReport> GetReportByIdAsync(int id);
    Task<List<FinancialReport>> GetReportsByTypeAsync(string reportType);
    Task<decimal> GetTotalInvoicedAmountAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalCollectedAmountAsync(DateTime startDate, DateTime endDate);
}
```

### DunningService

```csharp
public interface IDunningService
{
    Task<List<DunningNotice>> GetAllNoticesAsync();
    Task<DunningNotice> GetNoticeByIdAsync(int id);
    Task<List<DunningNotice>> GetNoticesByStudentAsync(int billingProfileId);
    Task<List<DunningNotice>> GetPendingNoticesAsync();
    Task<List<DunningNotice>> GetOverdueNoticesAsync();
    Task<DunningNotice> CreateDunningNoticeAsync(DunningNotice notice);
    Task UpdateNoticeAsync(DunningNotice notice);
    Task<string> GenerateNoticeNumberAsync();
    Task GenerateAutomaticDunningNoticesAsync();
    Task MarkNoticeAsResolvedAsync(int noticeId, string notes);
    Task EscalateNoticeAsync(int noticeId);
    Task<List<DunningNotice>> GetNoticesByLevelAsync(string level);
}
```

## 🔗 Database Relationships Diagram

```
┌─────────────────────────────────────────────────────────┐
│               Student (Existing)                          │
│  ├─ Id (PK)                                              │
│  ├─ StudentId (Unique)                                   │
│  └─ FullName, Email, Password                            │
└──────────────────┬──────────────────────────────────────┘
                   │ 1:1
                   ↓
┌────────────────────────────────────────────────────────┐
│        StudentBillingProfile (NEW Core Entity)          │
│  ├─ Id (PK)                                             │
│  ├─ StudentId (FK)                                      │
│  ├─ BillingStatus, TotalOutstanding, TotalPaid          │
│  ├─ CreditBalance, PaymentMethod, AutoPaymentEnabled    │
│  └─ BillingAddress, Phone, LastPaymentDate              │
└──┬──────────────────┬─────────────┬──────────────┬────┘
   │ 1:M              │ 1:M         │ 1:M          │ 1:M
   ↓                  ↓             ↓              ↓
┌──────────────────┐ ┌──────────┐ ┌─────────┐ ┌──────────────┐
│    Invoice       │ │ Payment  │ │ Refund  │ │Dunning Notice│
│  ├─ Id (PK)      │ ├─ Id (PK) │ ├─ Id (PK)│ ├─ Id (PK)    │
│  ├─ Student FK   │ ├─ Student│ ├─ Student│ ├─ Student FK │
│  ├─ Fee FK       │ ├─ Fee FK │ ├─ Payment│ ├─ Invoice FK │
│  ├─ Invoice#     │ ├─ Payment│ └─────────┘ └──────────────┘
│  ├─ Amount       │ │ Method  │
│  ├─ Tax, Discount│ ├─ Status │
│  ├─ DueDate      │ └─ Gateway│
│  ├─ Status       │
│  └─ LineItems    │
└──┬───────────────┘
   │ 1:M
   ↓
┌──────────────────┐
│ InvoiceLineItem  │
│  ├─ Id (PK)      │
│  ├─ Invoice FK   │
│  ├─ Description  │
│  └─ Amount       │
└──────────────────┘

┌─────────────────────────────────────┐
│        FeeStructure (NEW)           │
│  ├─ Id (PK)                         │
│  ├─ FeeType                         │
│  ├─ Amount, Currency                │
│  ├─ IsMandatory, IsRefundable       │
│  └─ FrequencyInMonths               │
└────────────────┬────────────────────┘
                 │ 1:M
                 ↓
            Invoice (has FK)

┌──────────────────────────────────────┐
│     PaymentGateway (NEW)             │
│  ├─ Id (PK)                          │
│  ├─ GatewayName                      │
│  ├─ ApiKey, SecretKey                │
│  ├─ TransactionFeePercent/Fixed      │
│  └─ SupportedCurrencies              │
└─────────────────┬──────────────────┘
                  │ 1:M
                  ↓
        Payment (has FK)

┌────────────────────────────────┐
│    PaymentTracking (NEW)       │
│  ├─ Id (PK)                    │
│  ├─ Payment FK                 │
│  ├─ Status                     │
│  ├─ BankReference              │
│  ├─ IsMatched, MatchedDate     │
│  └─ Details                    │
└────────────────────────────────┘

┌───────────────────────────────┐
│   FinancialReport (NEW)       │
│  ├─ Id (PK)                   │
│  ├─ ReportType                │
│  ├─ TotalInvoiced             │
│  ├─ TotalCollected            │
│  ├─ CollectionRate            │
│  └─ GeneratedDate             │
└───────────────────────────────┘
```

## 🎛️ BillingController Routes

```
GET  /Billing/Dashboard                    → Dashboard (Router)
GET  /Billing/StudentDashboard             → Student Dashboard
GET  /Billing/AdminDashboard               → Admin Dashboard

GET  /Billing/Invoices                     → Invoice List
GET  /Billing/InvoiceDetail/{id}           → Invoice Detail
GET  /Billing/CreateInvoice                → Create Invoice Form
POST /Billing/CreateInvoice                → Create Invoice

GET  /Billing/Payments                     → Payment List
GET  /Billing/PaymentDetail/{id}           → Payment Detail
GET  /Billing/MakePayment                  → Make Payment Form
POST /Billing/MakePayment                  → Process Payment
GET  /Billing/PaymentSuccess/{id}          → Payment Confirmation

GET  /Billing/Refunds                      → Refund List
GET  /Billing/RefundDetail/{id}            → Refund Detail
GET  /Billing/RequestRefund                → Request Refund Form
POST /Billing/RequestRefund                → Create Refund Request
POST /Billing/ApproveRefund/{id}           → Approve Refund
POST /Billing/RejectRefund                 → Reject Refund

GET  /Billing/Fees                         → Fee List
GET  /Billing/CreateFee                    → Create Fee Form
POST /Billing/CreateFee                    → Create Fee
GET  /Billing/EditFee/{id}                 → Edit Fee Form
POST /Billing/EditFee                      → Update Fee

GET  /Billing/Reports                      → Report List
GET  /Billing/GenerateReport               → Generate Report Form
POST /Billing/GenerateDailyReport          → Generate Daily Report
POST /Billing/GenerateMonthlyReport        → Generate Monthly Report
POST /Billing/GenerateAnnualReport         → Generate Annual Report
GET  /Billing/ReportDetail/{id}            → Report Detail

GET  /Billing/DunningNotices               → Dunning List
GET  /Billing/DunningNoticeDetail/{id}     → Dunning Detail
POST /Billing/GenerateAutomaticDunning     → Auto-Generate Notices
POST /Billing/EscalateDunning/{id}         → Escalate Notice
POST /Billing/ResolveDunning               → Resolve Notice

GET  /Billing/PaymentGateways              → Gateway List
GET  /Billing/CreatePaymentGateway         → Create Gateway Form
POST /Billing/CreatePaymentGateway         → Create Gateway
```

## 📊 Data Model Statistics

```
StudentBillingProfile
├─ 13 Properties
├─ 1 Foreign Key (Student)
└─ 3 Navigation Properties (Invoice, Payment, Refund)

FeeStructure
├─ 13 Properties
└─ 1 Navigation Property (Invoice)

Invoice
├─ 18 Properties
├─ 2 Foreign Keys (StudentBillingProfile, FeeStructure)
└─ 3 Navigation Properties (Payment, InvoiceLineItem, DunningNotice)

InvoiceLineItem
├─ 8 Properties
├─ 1 Foreign Key (Invoice)
└─ 0 Navigation Properties

Payment
├─ 16 Properties
├─ 3 Foreign Keys (StudentBillingProfile, Invoice, PaymentGateway)
└─ 2 Navigation Properties (PaymentTracking, Refund)

PaymentGateway
├─ 13 Properties
└─ 1 Navigation Property (Payment)

PaymentTracking
├─ 11 Properties
├─ 1 Foreign Key (Payment)
└─ 0 Navigation Properties

Refund
├─ 15 Properties
├─ 2 Foreign Keys (Payment, StudentBillingProfile)
└─ 0 Navigation Properties

FinancialReport
├─ 16 Properties
└─ 0 Foreign Keys

DunningNotice
├─ 18 Properties
├─ 2 Foreign Keys (StudentBillingProfile, Invoice)
└─ 0 Navigation Properties
```

---

This comprehensive component hierarchy shows the complete structure, relationships, and API surface of the Payment & Billing System.

