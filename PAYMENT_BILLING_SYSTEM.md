# Payment & Billing System Documentation

## Overview
A comprehensive payment and billing system has been integrated into the BMIT2023 application. This system manages student invoices, payments, refunds, fee structures, financial reports, and dunning (payment reminder) notices.

## System Architecture

### 1. Database Models

#### StudentBillingProfile
- Links each student to their billing information
- Tracks total outstanding balance, amount paid, and credit balance
- Stores payment preferences and contact information
- Fields: BillingStatus, TotalOutstanding, TotalPaid, CreditBalance, PaymentMethod, AutoPaymentEnabled

#### FeeStructure
- Defines different types of fees (tuition, lab, library, activity, sports, etc.)
- Supports multiple fee types with configurable amounts
- Fields: FeeType, Amount, Currency, IsMandatory, IsActive, FrequencyInMonths, IsRefundable

#### Invoice
- Represents bills sent to students
- Auto-generates invoice numbers (INV-YYYY-XXXXX format)
- Tracks invoice status (Pending, Issued, PartiallyPaid, Paid, Overdue, Cancelled)
- Supports line items, discounts, and taxes
- Fields: InvoiceNumber, Amount, DiscountAmount, TaxAmount, TotalAmount, DueDate, Status

#### InvoiceLineItem
- Detailed line items within an invoice
- Supports quantity, unit price, and per-line discounts
- Fields: ItemDescription, Quantity, UnitPrice, LineTotal, DiscountPercent

#### Payment
- Records payment transactions
- Supports multiple payment methods (CreditCard, DebitCard, BankTransfer, Cheque, Cash, OnlineGateway)
- Integrates with payment gateways
- Tracks reconciliation status
- Fields: PaymentReference, Amount, PaymentMethod, Status, TransactionId, IsReconciled

#### PaymentGateway
- Configures payment processors (Stripe, PayPal, Square, Bank Transfer, etc.)
- Stores API credentials and transaction fees
- Fields: GatewayName, ApiKey, SecretKey, TransactionFeePercent, SupportedCurrencies

#### PaymentTracking
- Tracks payment processing steps (Initiated, Authorized, Captured, Settled, Failed, Cancelled)
- Supports reconciliation with bank records
- Fields: Status, BankReferenceNumber, BankClearingDate, IsMatched

#### Refund
- Manages refund requests and processing
- Supports multiple refund methods (Original, BankTransfer, Credit)
- Tracks approval workflow
- Fields: RefundReference, RefundAmount, RefundReason, Status, RefundMethod

#### FinancialReport
- Generates comprehensive financial reports
- Supports multiple report types (DailyRevenue, MonthlyRevenue, AnnualRevenue, OutstandingInvoices, StudentAging, FeeCollection)
- Fields: ReportType, TotalInvoiced, TotalCollected, TotalOutstanding, CollectionRate

#### DunningNotice
- Manages payment reminders and escalation
- Four escalation levels (Level1: 7 days, Level2: 15 days, Level3: 30 days, Level4: 60 days overdue)
- Auto-generates notices for overdue invoices
- Fields: NoticeNumber, NoticeLevel, OutstandingAmount, Status, LateFeeCharged

### 2. Services (Business Logic)

#### IStudentBillingProfileService
```csharp
- GetProfileByIdAsync(int id)
- GetProfileByStudentIdAsync(int studentId)
- CreateProfileAsync(StudentBillingProfile profile)
- UpdateProfileAsync(StudentBillingProfile profile)
- GetAllProfilesAsync()
- UpdateProfileStatusAsync(int profileId, string status)
```

#### IFeeStructureService
```csharp
- GetAllFeesAsync()
- GetActiveFeesAsync()
- GetMandatoryFeesAsync()
- CreateFeeAsync(FeeStructure fee)
- UpdateFeeAsync(FeeStructure fee)
- DeleteFeeAsync(int id)
- CalculateTotalFeesForStudentAsync(int studentId)
```

#### IInvoiceService
```csharp
- GetAllInvoicesAsync()
- GetInvoicesByStudentAsync(int billingProfileId)
- GetOverdueInvoicesAsync()
- GetPendingInvoicesAsync()
- CreateInvoiceAsync(Invoice invoice)
- UpdateInvoiceAsync(Invoice invoice)
- GenerateInvoiceNumberAsync()
- UpdateInvoiceStatusAsync(int invoiceId, string status)
```

#### IPaymentService
```csharp
- GetAllPaymentsAsync()
- GetPaymentsByStudentAsync(int billingProfileId)
- GetPaymentsByInvoiceAsync(int invoiceId)
- GetPendingPaymentsAsync()
- CreatePaymentAsync(Payment payment)
- ProcessPaymentAsync(int paymentId)
- CalculateGatewayFeesAsync(decimal amount, int gatewayId)
- GetUnreconciledPaymentsAsync()
```

#### IRefundService
```csharp
- GetAllRefundsAsync()
- GetRefundsByStudentAsync(int billingProfileId)
- GetPendingRefundsAsync()
- GetApprovedRefundsAsync()
- CreateRefundAsync(Refund refund)
- ApproveRefundAsync(int refundId, int adminId, string notes)
- RejectRefundAsync(int refundId, string reason)
- ProcessRefundAsync(int refundId)
```

#### IFinancialReportService
```csharp
- GenerateDailyRevenueReportAsync(DateTime date, int adminId)
- GenerateMonthlyRevenueReportAsync(int year, int month, int adminId)
- GenerateAnnualRevenueReportAsync(int year, int adminId)
- GenerateOutstandingInvoicesReportAsync(int adminId)
- GenerateStudentAgingReportAsync(int adminId)
- GenerateFeeCollectionReportAsync(DateTime startDate, DateTime endDate, int adminId)
- GetAllReportsAsync()
```

#### IDunningService
```csharp
- GetAllNoticesAsync()
- GetNoticesByStudentAsync(int billingProfileId)
- GetPendingNoticesAsync()
- GetOverdueNoticesAsync()
- CreateDunningNoticeAsync(DunningNotice notice)
- GenerateAutomaticDunningNoticesAsync()
- MarkNoticeAsResolvedAsync(int noticeId, string notes)
- EscalateNoticeAsync(int noticeId)
```

### 3. BillingController Actions

#### Dashboard Views
- **Dashboard**: Main billing dashboard (routes to Student or Admin dashboard)
- **StudentDashboard**: Shows student's billing info, outstanding balance, invoices
- **AdminDashboard**: Shows summary statistics, pending payments, overdue invoices

#### Invoice Management
- **Invoices**: List all invoices
- **InvoiceDetail**: View invoice details, payments, and line items
- **CreateInvoice**: Create new invoice for a student

#### Payment Management
- **Payments**: List all payments with status
- **PaymentDetail**: View payment details and tracking
- **MakePayment**: Record/process a payment
- **PaymentSuccess**: Confirmation page after payment

#### Refund Management
- **Refunds**: List all refund requests
- **RefundDetail**: View refund details
- **RequestRefund**: Request a refund for a payment
- **ApproveRefund**: Admin action to approve refund
- **RejectRefund**: Admin action to reject refund

#### Fee Management
- **Fees**: List all fee structures
- **CreateFee**: Create new fee structure
- **EditFee**: Edit existing fee structure

#### Financial Reports
- **Reports**: List generated reports
- **GenerateReport**: Form to generate various report types
- **ReportDetail**: View detailed report

#### Dunning Management
- **DunningNotices**: List all dunning notices
- **DunningNoticeDetail**: View notice details
- **GenerateAutomaticDunning**: Generate notices for overdue invoices
- **EscalateDunning**: Escalate a notice level
- **ResolveDunning**: Mark notice as resolved

#### Payment Gateway Management
- **PaymentGateways**: List configured gateways
- **CreatePaymentGateway**: Add new payment gateway

### 4. Views (User Interface)

#### Billing Views Location: `/Views/Billing/`

- **Dashboard.cshtml**: Main dashboard router
- **StudentDashboard.cshtml**: Student view with balance, recent invoices
- **AdminDashboard.cshtml**: Admin view with summary stats
- **Invoices.cshtml**: Invoice list table
- **InvoiceDetail.cshtml**: Detailed invoice view
- **CreateInvoice.cshtml**: Invoice creation form
- **Payments.cshtml**: Payment records list
- **MakePayment.cshtml**: Payment entry form
- **Refunds.cshtml**: Refund requests list
- **Fees.cshtml**: Fee structures list
- **CreateFee.cshtml**: Fee creation form
- **Reports.cshtml**: Generated reports list
- **GenerateReport.cshtml**: Report generation form
- **DunningNotices.cshtml**: Dunning notices list

## Key Features

### 1. Invoice Generation
- Auto-numbered invoices (INV-YYYY-XXXXX)
- Support for line items with individual discounts
- Tax and discount calculation
- Status tracking (Pending → Issued → PartiallyPaid/Paid/Overdue)
- Recurring invoice support

### 2. Fee Structure Management
- Multiple fee types (Tuition, Lab, Library, Activity, Sports, etc.)
- Mandatory vs. optional fees
- One-time or recurring fees (monthly, yearly, custom frequency)
- Refundable flag for each fee
- Effective date ranges

### 3. Payment Gateway Integration
- Support for multiple payment gateways:
  - Stripe (2.9% + $0.30)
  - PayPal (3.49% + $0.49)
  - Bank Transfer ($5 fixed)
  - Cash/Check (free)
- Configurable transaction fees
- Currency support
- Min/Max amount limits
- Status: Pending → Processing → Successful/Failed

### 4. Payment Tracking & Reconciliation
- Multi-step payment tracking
- Payment status workflow
- Bank reference matching
- Clearing date tracking
- Reconciliation report

### 5. Refund & Credit Management
- Multiple refund methods:
  - Original payment method
  - Bank transfer
  - Store credit
- Approval workflow
- Rejection with reasons
- Automatic credit balance updates
- Processing status tracking

### 6. Financial Reporting
- **Daily Revenue Reports**: Daily invoice and payment totals
- **Monthly Revenue Reports**: Monthly breakdown with collection rates
- **Annual Revenue Reports**: Year-over-year analysis with refunds
- **Outstanding Invoices Report**: All unpaid invoices
- **Student Aging Report**: Overdue invoice analysis
- **Fee Collection Report**: Fee-specific collection metrics
- Metrics include: Total invoiced, collected, outstanding, collection rate

### 7. Automated Reminders & Dunning
- **4-Level Escalation System**:
  - Level 1: 7 days overdue (Reminder)
  - Level 2: 15 days overdue (Warning)
  - Level 3: 30 days overdue (Escalation)
  - Level 4: 60 days overdue (Final Notice)
- Auto-generation of notices
- Multiple notification methods (Email, SMS, Phone, Mail)
- Late fees support
- Manual escalation option
- Resolution tracking

## Database Setup

### Seed Data
The system comes pre-configured with:
- 5 default fee structures (Tuition, Lab, Library, Activity, Sports)
- 4 payment gateways (Stripe, PayPal, Bank Transfer, Cash/Check)

### Migration
Run the migration to create all tables:
```bash
dotnet ef database update
```

## Login & Access

### Student Login
1. Login at `/Account/Login`
2. Redirected to `/Billing/Dashboard` (StudentDashboard)
3. View invoices, make payments, request refunds

### Admin Login
1. Login at `/Account/Login`
2. Redirected to `/Billing/Dashboard` (AdminDashboard)
3. Manage all billing functions

## Integration with Existing System

### AccountController
- **Modified**: Login now redirects to `/Billing/Dashboard` instead of `/Home/Index`
- Maintains UserRole and UserId in TempData

### HomeController
- **Modified**: Index redirects to `/Billing/Dashboard` if user is logged in
- Preserves Privacy and Error actions

### Program.cs
- **Added**: Service registrations for all billing services
- Maintains existing DbContext configuration

## Auto-Numbering Format
- Invoices: `INV-2025-00001`
- Payments: `PAY-2025-00001`
- Refunds: `REF-2025-00001`
- Dunning Notices: `DN-2025-00001`

## Status Enums & Workflows

### Invoice Status
Pending → Issued → PartiallyPaid/Paid/Overdue → Cancelled (optional)

### Payment Status
Pending → Processing → Successful/Failed → Cancelled/Refunded

### Refund Status
Pending → Approved → Processing → Completed/Rejected

### Dunning Notice Status
Sent → Read/Acknowledged → Escalated → Resolved

## Future Enhancements
1. Email notification integration
2. Payment gateway API integration
3. Automated recurring invoice generation
4. Late fee auto-application
5. Student payment plans
6. Accounting GL integration
7. Tax calculation engine
8. Multi-currency support enhancement
9. Payment reminder scheduling
10. Advanced financial analytics

## Testing Workflow

1. **Register a Student**
   - Go to `/Account/Register`
   - Create a student account

2. **Create Fee Structure**
   - Login as Admin
   - Go to `/Billing/Fees` → Create Fee
   - Create sample fees

3. **Create Invoice**
   - Go to `/Billing/CreateInvoice`
   - Select student and fee
   - Set amount and due date

4. **Make Payment**
   - Go to `/Billing/MakePayment`
   - Select student and invoice
   - Enter amount and payment method

5. **Generate Report**
   - Go to `/Billing/GenerateReport`
   - Select report type and parameters
   - View generated report

6. **Manage Dunning**
   - Go to `/Billing/DunningNotices`
   - Click "Generate Automatic Notices" for overdue invoices
   - Escalate notices as needed

## Security Considerations
1. Implement role-based authorization (requires [Authorize] attributes)
2. Validate payment amounts before processing
3. Encrypt sensitive payment gateway credentials
4. Use parameterized queries (already using EF Core)
5. Log all financial transactions
6. Implement audit trails
7. Use HTTPS for all payments
8. Implement CSRF protection (add token validation)

## Performance Notes
- Use pagination for large invoice/payment lists
- Consider caching fee structures
- Index on StudentBillingProfileId, InvoiceId, PaymentId
- Lazy load navigation properties as needed

## Database Relationships
```
Student ←→ StudentBillingProfile (1:1)
StudentBillingProfile ←→ Invoice (1:Many)
StudentBillingProfile ←→ Payment (1:Many)
StudentBillingProfile ←→ Refund (1:Many)
StudentBillingProfile ←→ DunningNotice (1:Many)
FeeStructure ←→ Invoice (1:Many)
Invoice ←→ InvoiceLineItem (1:Many)
Invoice ←→ Payment (1:Many)
Invoice ←→ DunningNotice (1:Many)
Payment ←→ PaymentGateway (Many:1)
Payment ←→ Refund (1:Many)
Payment ←→ PaymentTracking (1:Many)
PaymentGateway ←→ Payment (1:Many)
```

## Support & Maintenance
For issues or questions:
1. Check database logs for errors
2. Verify service registrations in Program.cs
3. Ensure all migrations are applied
4. Check TempData for user context
5. Review validation errors in ModelState
