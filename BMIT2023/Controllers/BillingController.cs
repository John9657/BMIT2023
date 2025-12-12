using Microsoft.AspNetCore.Mvc;
using BMIT2023.Models;
using BMIT2023.Data;
using BMIT2023.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Controllers
{
    public class BillingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IStudentBillingProfileService _billingProfileService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly IRefundService _refundService;
        private readonly IFinancialReportService _reportService;
        private readonly IDunningService _dunningService;
        private readonly IFeeStructureService _feeService;

        public BillingController(
            AppDbContext context,
            IStudentBillingProfileService billingProfileService,
            IInvoiceService invoiceService,
            IPaymentService paymentService,
            IRefundService refundService,
            IFinancialReportService reportService,
            IDunningService dunningService,
            IFeeStructureService feeService)
        {
            _context = context;
            _billingProfileService = billingProfileService;
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _refundService = refundService;
            _reportService = reportService;
            _dunningService = dunningService;
            _feeService = feeService;
        }

        // ==================== DASHBOARD ====================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = TempData["UserId"]?.ToString();
                var userRole = TempData["UserRole"]?.ToString();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
                {
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.UserRole = userRole;
                ViewBag.UserId = userId;

                if (userRole == "Student")
                {
                    return await StudentDashboard(userId);
                }
                else if (userRole == "Admin")
                {
                    return await AdminDashboard();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading dashboard: {ex.Message}";
                return View();
            }
        }

        private async Task<IActionResult> StudentDashboard(string studentId)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var billingProfile = await _billingProfileService.GetProfileByStudentIdAsync(student.Id);
                if (billingProfile == null)
                {
                    // Create billing profile if doesn't exist
                    billingProfile = new StudentBillingProfile
                    {
                        StudentId = student.Id,
                        BillingStatus = "Active",
                        Phone = "",
                        BillingAddress = ""
                    };
                    await _billingProfileService.CreateProfileAsync(billingProfile);
                }

                ViewBag.BillingProfile = billingProfile;
                ViewBag.Student = student;
                ViewBag.InvoiceCount = billingProfile.Invoices?.Count ?? 0;
                ViewBag.OutstandingAmount = billingProfile.TotalOutstanding;
                ViewBag.CreditBalance = billingProfile.CreditBalance;

                return View("StudentDashboard", billingProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading student dashboard: {ex.Message}";
                return View("StudentDashboard");
            }
        }

        private async Task<IActionResult> AdminDashboard()
        {
            try
            {
                var totalInvoiced = await _reportService.GetTotalInvoicedAmountAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                var totalCollected = await _reportService.GetTotalCollectedAmountAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                var pendingPayments = await _paymentService.GetPendingPaymentsAsync();
                var pendingRefunds = await _refundService.GetPendingRefundsAsync();
                var overdueInvoices = await _invoiceService.GetOverdueInvoicesAsync();

                ViewBag.TotalInvoiced = totalInvoiced;
                ViewBag.TotalCollected = totalCollected;
                ViewBag.PendingPaymentCount = pendingPayments.Count;
                ViewBag.PendingRefundCount = pendingRefunds.Count;
                ViewBag.OverdueInvoiceCount = overdueInvoices.Count;

                return View("AdminDashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading admin dashboard: {ex.Message}";
                return View("AdminDashboard");
            }
        }

        // ==================== INVOICE MANAGEMENT ====================
        [HttpGet]
        public async Task<IActionResult> Invoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceDetail(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> CreateInvoice()
        {
            ViewBag.Students = await _billingProfileService.GetAllProfilesAsync();
            ViewBag.Fees = await _feeService.GetActiveFeesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    invoice.Status = "Issued";
                    invoice.AmountDue = invoice.TotalAmount;
                    await _invoiceService.CreateInvoiceAsync(invoice);
                    return RedirectToAction(nameof(Invoices));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating invoice: {ex.Message}");
                }
            }
            ViewBag.Students = await _billingProfileService.GetAllProfilesAsync();
            ViewBag.Fees = await _feeService.GetActiveFeesAsync();
            return View(invoice);
        }

        // ==================== PAYMENT MANAGEMENT ====================
        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentDetail(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        [HttpGet]
        public async Task<IActionResult> MakePayment()
        {
            ViewBag.Invoices = await _invoiceService.GetPendingInvoicesAsync();
            ViewBag.PaymentGateways = _context.PaymentGateways.Where(p => p.IsActive).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    payment.Status = "Pending";
                    await _paymentService.CreatePaymentAsync(payment);
                    await _paymentService.ProcessPaymentAsync(payment.Id);
                    return RedirectToAction(nameof(PaymentSuccess), new { id = payment.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error processing payment: {ex.Message}");
                }
            }
            ViewBag.Invoices = await _invoiceService.GetPendingInvoicesAsync();
            ViewBag.PaymentGateways = _context.PaymentGateways.Where(p => p.IsActive).ToList();
            return View(payment);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // ==================== REFUND MANAGEMENT ====================
        [HttpGet]
        public async Task<IActionResult> Refunds()
        {
            var refunds = await _refundService.GetAllRefundsAsync();
            return View(refunds);
        }

        [HttpGet]
        public async Task<IActionResult> RefundDetail(int id)
        {
            var refund = await _refundService.GetRefundByIdAsync(id);
            if (refund == null)
            {
                return NotFound();
            }
            return View(refund);
        }

        [HttpGet]
        public async Task<IActionResult> RequestRefund()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments.Where(p => p.Status == "Successful").ToList());
        }

        [HttpPost]
        public async Task<IActionResult> RequestRefund(Refund refund)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _refundService.CreateRefundAsync(refund);
                    return RedirectToAction(nameof(Refunds));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating refund request: {ex.Message}");
                }
            }
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments.Where(p => p.Status == "Successful").ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRefund(int id)
        {
            try
            {
                await _refundService.ApproveRefundAsync(id, 1, "Approved by admin");
                return RedirectToAction(nameof(Refunds));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving refund: {ex.Message}";
                return RedirectToAction(nameof(Refunds));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectRefund(int id, string reason)
        {
            try
            {
                await _refundService.RejectRefundAsync(id, reason);
                return RedirectToAction(nameof(Refunds));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting refund: {ex.Message}";
                return RedirectToAction(nameof(Refunds));
            }
        }

        // ==================== FEE MANAGEMENT ====================
        [HttpGet]
        public async Task<IActionResult> Fees()
        {
            var fees = await _feeService.GetAllFeesAsync();
            return View(fees);
        }

        [HttpGet]
        public IActionResult CreateFee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFee(FeeStructure fee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _feeService.CreateFeeAsync(fee);
                    return RedirectToAction(nameof(Fees));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating fee: {ex.Message}");
                }
            }
            return View(fee);
        }

        [HttpGet]
        public async Task<IActionResult> EditFee(int id)
        {
            var fee = await _feeService.GetFeeByIdAsync(id);
            if (fee == null)
            {
                return NotFound();
            }
            return View(fee);
        }

        [HttpPost]
        public async Task<IActionResult> EditFee(FeeStructure fee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _feeService.UpdateFeeAsync(fee);
                    return RedirectToAction(nameof(Fees));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating fee: {ex.Message}");
                }
            }
            return View(fee);
        }

        // ==================== FINANCIAL REPORTS ====================
        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return View(reports);
        }

        [HttpGet]
        public IActionResult GenerateReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDailyReport(DateTime date)
        {
            try
            {
                var report = await _reportService.GenerateDailyRevenueReportAsync(date, 1);
                return RedirectToAction(nameof(ReportDetail), new { id = report.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating report: {ex.Message}";
                return RedirectToAction(nameof(GenerateReport));
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateMonthlyReport(int year, int month)
        {
            try
            {
                var report = await _reportService.GenerateMonthlyRevenueReportAsync(year, month, 1);
                return RedirectToAction(nameof(ReportDetail), new { id = report.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating report: {ex.Message}";
                return RedirectToAction(nameof(GenerateReport));
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAnnualReport(int year)
        {
            try
            {
                var report = await _reportService.GenerateAnnualRevenueReportAsync(year, 1);
                return RedirectToAction(nameof(ReportDetail), new { id = report.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating report: {ex.Message}";
                return RedirectToAction(nameof(GenerateReport));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReportDetail(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        // ==================== DUNNING MANAGEMENT ====================
        [HttpGet]
        public async Task<IActionResult> DunningNotices()
        {
            var notices = await _dunningService.GetAllNoticesAsync();
            return View(notices);
        }

        [HttpGet]
        public async Task<IActionResult> DunningNoticeDetail(int id)
        {
            var notice = await _dunningService.GetNoticeByIdAsync(id);
            if (notice == null)
            {
                return NotFound();
            }
            return View(notice);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAutomaticDunning()
        {
            try
            {
                await _dunningService.GenerateAutomaticDunningNoticesAsync();
                TempData["Success"] = "Automatic dunning notices generated successfully";
                return RedirectToAction(nameof(DunningNotices));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating dunning notices: {ex.Message}";
                return RedirectToAction(nameof(DunningNotices));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EscalateDunning(int id)
        {
            try
            {
                await _dunningService.EscalateNoticeAsync(id);
                TempData["Success"] = "Notice escalated successfully";
                return RedirectToAction(nameof(DunningNoticeDetail), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error escalating notice: {ex.Message}";
                return RedirectToAction(nameof(DunningNoticeDetail), new { id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResolveDunning(int id, string notes)
        {
            try
            {
                await _dunningService.MarkNoticeAsResolvedAsync(id, notes);
                TempData["Success"] = "Notice marked as resolved";
                return RedirectToAction(nameof(DunningNotices));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error resolving notice: {ex.Message}";
                return RedirectToAction(nameof(DunningNoticeDetail), new { id });
            }
        }

        // ==================== PAYMENT GATEWAY MANAGEMENT ====================
        [HttpGet]
        public IActionResult PaymentGateways()
        {
            var gateways = _context.PaymentGateways.ToList();
            return View(gateways);
        }

        [HttpGet]
        public IActionResult CreatePaymentGateway()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePaymentGateway(PaymentGateway gateway)
        {
            if (ModelState.IsValid)
            {
                gateway.CreatedDate = DateTime.Now;
                _context.PaymentGateways.Add(gateway);
                _context.SaveChanges();
                return RedirectToAction(nameof(PaymentGateways));
            }
            return View(gateway);
        }
    }
}
