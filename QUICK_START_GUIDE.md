# Quick Start Guide - Payment & Billing System

## Prerequisites
- .NET 9.0
- SQL Server
- Visual Studio Code with C# extension

## Installation Steps

### 1. Apply Database Migration
```bash
cd c:\Users\johnl\OneDrive\文档\GitHub\BMIT2023\BMIT2023
dotnet ef database update
```

This will create all 10 database tables and seed the initial data:
- 5 fee structures
- 4 payment gateways

### 2. Run the Application
```bash
dotnet run
```

The application will start at `https://localhost:5001`

## First Time Usage

### As an Admin User

1. **Login**
   - Navigate to `https://localhost:5001/Account/Login`
   - Use credentials: `AdminId: A001`, `Password: password123`
   - You'll be redirected to `/Billing/Dashboard` → Admin Dashboard

2. **Create a Fee Structure** (if needed)
   - Click "Manage Fees" or go to `/Billing/Fees`
   - Click "Add New Fee"
   - Fill in fee type, description, amount
   - Save

3. **Create an Invoice**
   - Go to `/Billing/CreateInvoice`
   - Select a student
   - Select a fee type
   - Set amount and due date
   - Click "Create Invoice"

4. **View All Invoices**
   - Go to `/Billing/Invoices`
   - See list of all invoices with status

5. **Record a Payment**
   - Go to `/Billing/Payments`
   - Click "Record Payment"
   - Select student and invoice
   - Enter amount and payment method
   - Process payment

### As a Student User

1. **Register**
   - Go to `https://localhost:5001/Account/Register`
   - Fill in Full Name, Email, Password
   - Your Student ID will be auto-generated
   - Note your Student ID

2. **Login**
   - Use your auto-generated Student ID and password
   - Redirected to Student Dashboard

3. **View Your Invoices**
   - See outstanding balance, credit balance, and total paid
   - View recent invoices table
   - Click "View All Invoices" for complete list
   - Click "Details" on any invoice to see full breakdown

4. **Make a Payment**
   - Click "Make Payment" button
   - Select an invoice
   - Enter payment amount
   - Select payment method
   - Submit

5. **Request a Refund**
   - Click "Request Refund"
   - Select a payment to refund
   - Enter reason
   - Submit for admin approval

## Key URLs

```
/Billing/Dashboard              - Main billing dashboard (redirects based on role)
/Billing/StudentDashboard       - Student's personal billing view
/Billing/AdminDashboard         - Admin overview with statistics

/Billing/Invoices               - List all invoices
/Billing/InvoiceDetail/{id}     - View specific invoice
/Billing/CreateInvoice          - Create new invoice

/Billing/Payments               - List all payments
/Billing/PaymentDetail/{id}     - View specific payment
/Billing/MakePayment            - Record a payment

/Billing/Refunds                - List all refunds
/Billing/RefundDetail/{id}      - View specific refund
/Billing/RequestRefund          - Request a refund

/Billing/Fees                   - List fee structures
/Billing/CreateFee              - Create new fee
/Billing/EditFee/{id}           - Edit fee structure

/Billing/Reports                - List generated reports
/Billing/GenerateReport         - Generate new report
/Billing/ReportDetail/{id}      - View report details

/Billing/DunningNotices         - List dunning notices
/Billing/DunningNoticeDetail/{id} - View notice details
```

## Common Tasks

### Generate Invoices for All Students
```csharp
// In BillingController.CreateInvoice POST action
// Call InvoiceService for each student
```

### Auto-Generate Dunning Notices
1. Go to `/Billing/DunningNotices`
2. Click "Generate Automatic Notices"
3. System creates notices for all overdue invoices

### Generate Financial Report
1. Go to `/Billing/GenerateReport`
2. Choose report type:
   - Daily: Select date
   - Monthly: Select year and month
   - Annual: Select year
3. Click "Generate"
4. View report in `/Billing/Reports`

### Process a Refund
1. Go to `/Billing/Refunds`
2. Click "Details" on pending refund
3. Click "Approve" or "Reject"
4. If approved, it enters processing
5. Can view in refunds list

## Sample Data Flow

### Scenario 1: Student Gets Invoiced and Pays
1. Admin creates invoice for Student S001
2. Student S001 logs in, sees invoice
3. Student clicks "Make Payment"
4. Payment recorded and processed
5. Invoice status changes from "Issued" to "Paid"
6. Student credit balance updated

### Scenario 2: Overdue Invoice & Dunning
1. Invoice due date passes without payment
2. Admin generates automatic dunning notices
3. Level 1 notice created (7+ days)
4. If still unpaid, manually escalate to Level 2
5. Continue escalation as needed
6. Once paid, resolve the notice

### Scenario 3: Financial Reporting
1. Admin goes to GenerateReport
2. Generates monthly report for November 2025
3. Report shows:
   - Total invoiced: $X
   - Total collected: $Y
   - Outstanding: $Z
   - Collection rate: %
4. Can view and analyze trends

## Troubleshooting

### Database Not Creating Tables
**Solution**: Make sure migration was applied correctly
```bash
dotnet ef database update --verbose
```

### Login Redirects to Home Instead of Billing
**Solution**: Check that AccountController.Login() redirects to "Billing", "Dashboard"
```csharp
return RedirectToAction("Dashboard", "Billing");
```

### Services Not Found Error
**Solution**: Verify services are registered in Program.cs
```csharp
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
// etc.
```

### Views Not Loading
**Solution**: Ensure views are in `/Views/Billing/` folder
```
Views/
  Billing/
    Dashboard.cshtml
    StudentDashboard.cshtml
    // etc.
```

### Payment Not Processing
**Solution**: Check PaymentGateway is configured correctly
```csharp
// Verify payment gateway exists in database
var gateway = await _context.PaymentGateways.FindAsync(paymentGatewayId);
```

## Development Tips

### Debug Mode
Set breakpoints in services to trace data flow:
1. Set breakpoint in InvoiceService.CreateInvoiceAsync()
2. Run in debug mode
3. Step through code

### Database Queries
View actual SQL in Output window (SQL Server logs)

### View Student Data
```csharp
var profile = await _billingProfileService.GetProfileByStudentIdAsync(studentId);
Console.WriteLine($"Outstanding: {profile.TotalOutstanding}");
```

### Check Invoice Status
```csharp
var invoices = await _invoiceService.GetInvoicesByStudentAsync(billingProfileId);
foreach(var inv in invoices)
{
    Console.WriteLine($"{inv.InvoiceNumber}: {inv.Status}");
}
```

## Next Steps

1. **Customize Fee Types**: Adjust default fees in seed data
2. **Add Email Notifications**: Implement email service for dunning notices
3. **Integrate Payment Gateways**: Add Stripe/PayPal API keys
4. **Add Authorization**: Protect endpoints with [Authorize] attributes
5. **Create Reports**: Build custom financial analysis reports
6. **Setup Automation**: Schedule automatic dunning notice generation
7. **Add Audit Logging**: Log all financial transactions

## Support Resources

- **Documentation**: See `PAYMENT_BILLING_SYSTEM.md`
- **Implementation**: See `IMPLEMENTATION_SUMMARY.md`
- **Code**: Check individual service classes for method documentation
- **Database**: Review `Migrations/20251213_PaymentBillingSystem.cs`

## Production Checklist

- [ ] Database backup configured
- [ ] Payment gateway APIs integrated
- [ ] Email notifications implemented
- [ ] Authorization attributes added
- [ ] Audit logging implemented
- [ ] Error handling tested
- [ ] UI/UX tested with real data
- [ ] Performance optimized
- [ ] Security audit completed
- [ ] Documentation updated
- [ ] Training completed

---

**You're all set! Start using the billing system now.** 🚀
