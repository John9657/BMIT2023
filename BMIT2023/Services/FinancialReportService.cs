using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class FinancialReportService : IFinancialReportService
    {
        private readonly AppDbContext _context;

        public FinancialReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FinancialReport> GenerateDailyRevenueReportAsync(DateTime date, int adminId)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1);

            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate < endDate)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate && p.Status == "Successful")
                .ToListAsync();

            var report = new FinancialReport
            {
                ReportName = $"Daily Revenue Report - {date:yyyy-MM-dd}",
                ReportType = "DailyRevenue",
                FromDate = startDate,
                ToDate = endDate,
                TotalInvoiced = invoices.Sum(i => i.TotalAmount),
                TotalCollected = payments.Sum(p => p.Amount),
                TotalOutstanding = invoices.Where(i => i.Status != "Paid").Sum(i => i.AmountDue),
                TotalInvoiceCount = invoices.Count,
                TotalPaidInvoiceCount = invoices.Count(i => i.Status == "Paid"),
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            CalculateReportMetrics(report);

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<FinancialReport> GenerateMonthlyRevenueReportAsync(int year, int month, int adminId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate < endDate)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate && p.Status == "Successful")
                .ToListAsync();

            var report = new FinancialReport
            {
                ReportName = $"Monthly Revenue Report - {year}-{month:00}",
                ReportType = "MonthlyRevenue",
                FromDate = startDate,
                ToDate = endDate,
                TotalInvoiced = invoices.Sum(i => i.TotalAmount),
                TotalCollected = payments.Sum(p => p.Amount),
                TotalOutstanding = invoices.Where(i => i.Status != "Paid").Sum(i => i.AmountDue),
                TotalInvoiceCount = invoices.Count,
                TotalPaidInvoiceCount = invoices.Count(i => i.Status == "Paid"),
                TotalOverdueInvoiceCount = invoices.Count(i => i.Status == "Overdue" || (DateTime.Now > i.DueDate && i.Status != "Paid")),
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            CalculateReportMetrics(report);

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<FinancialReport> GenerateAnnualRevenueReportAsync(int year, int adminId)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31).AddDays(1);

            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate < endDate)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate && p.Status == "Successful")
                .ToListAsync();

            var refunds = await _context.Refunds
                .Where(r => r.RequestedDate >= startDate && r.RequestedDate < endDate && r.Status == "Completed")
                .ToListAsync();

            var report = new FinancialReport
            {
                ReportName = $"Annual Revenue Report - {year}",
                ReportType = "AnnualRevenue",
                FromDate = startDate,
                ToDate = endDate,
                TotalInvoiced = invoices.Sum(i => i.TotalAmount),
                TotalCollected = payments.Sum(p => p.Amount),
                TotalRefunded = refunds.Sum(r => r.RefundAmount),
                TotalOutstanding = invoices.Where(i => i.Status != "Paid").Sum(i => i.AmountDue),
                TotalInvoiceCount = invoices.Count,
                TotalPaidInvoiceCount = invoices.Count(i => i.Status == "Paid"),
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            CalculateReportMetrics(report);

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<FinancialReport> GenerateOutstandingInvoicesReportAsync(int adminId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                .ToListAsync();

            var report = new FinancialReport
            {
                ReportName = "Outstanding Invoices Report",
                ReportType = "OutstandingInvoices",
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now,
                TotalOutstanding = invoices.Sum(i => i.AmountDue),
                TotalInvoiceCount = invoices.Count,
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<FinancialReport> GenerateStudentAgingReportAsync(int adminId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.Status != "Paid")
                .Include(i => i.StudentBillingProfile)
                .ToListAsync();

            var overdueInvoices = invoices.Where(i => DateTime.Now > i.DueDate).ToList();

            var report = new FinancialReport
            {
                ReportName = "Student Aging Report",
                ReportType = "StudentAging",
                FromDate = invoices.Min(i => i.InvoiceDate),
                ToDate = DateTime.Now,
                TotalOutstanding = invoices.Sum(i => i.AmountDue),
                TotalOverdueInvoiceCount = overdueInvoices.Count,
                TotalInvoiceCount = invoices.Count,
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<FinancialReport> GenerateFeeCollectionReportAsync(DateTime startDate, DateTime endDate, int adminId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .Include(i => i.FeeStructure)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.Status == "Successful")
                .ToListAsync();

            var report = new FinancialReport
            {
                ReportName = $"Fee Collection Report - {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                ReportType = "FeeCollection",
                FromDate = startDate,
                ToDate = endDate,
                TotalInvoiced = invoices.Sum(i => i.TotalAmount),
                TotalCollected = payments.Sum(p => p.Amount),
                TotalOutstanding = invoices.Where(i => i.Status != "Paid").Sum(i => i.AmountDue),
                TotalInvoiceCount = invoices.Count,
                TotalPaidInvoiceCount = invoices.Count(i => i.Status == "Paid"),
                GeneratedDate = DateTime.Now,
                GeneratedByAdminId = adminId
            };

            CalculateReportMetrics(report);

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<List<FinancialReport>> GetAllReportsAsync()
        {
            return await _context.FinancialReports
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        public async Task<FinancialReport> GetReportByIdAsync(int id)
        {
            return await _context.FinancialReports.FindAsync(id);
        }

        public async Task<List<FinancialReport>> GetReportsByTypeAsync(string reportType)
        {
            return await _context.FinancialReports
                .Where(r => r.ReportType == reportType)
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInvoicedAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<decimal> GetTotalCollectedAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.Status == "Successful")
                .SumAsync(p => p.Amount);
        }

        private void CalculateReportMetrics(FinancialReport report)
        {
            if (report.TotalInvoiced > 0)
            {
                report.CollectionRate = (report.TotalCollected / report.TotalInvoiced) * 100;
                report.AverageInvoiceAmount = report.TotalInvoiced / report.TotalInvoiceCount;
            }
        }
    }
}
