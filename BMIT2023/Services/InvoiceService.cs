using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.StudentBillingProfile)
                .Include(i => i.FeeStructure)
                .Include(i => i.Payments)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.StudentBillingProfile)
                .Include(i => i.FeeStructure)
                .Include(i => i.Payments)
                .Include(i => i.LineItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Invoice>> GetInvoicesByStudentAsync(int billingProfileId)
        {
            return await _context.Invoices
                .Where(i => i.StudentBillingProfileId == billingProfileId)
                .Include(i => i.FeeStructure)
                .Include(i => i.Payments)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetOverdueInvoicesAsync()
        {
            return await _context.Invoices
                .Where(i => i.Status == "Overdue" || (DateTime.Now > i.DueDate && i.Status != "Paid"))
                .Include(i => i.StudentBillingProfile)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetPendingInvoicesAsync()
        {
            return await _context.Invoices
                .Where(i => i.Status == "Pending" || i.Status == "Issued")
                .Include(i => i.StudentBillingProfile)
                .ToListAsync();
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
            invoice.InvoiceDate = DateTime.Now;
            invoice.AmountDue = invoice.TotalAmount - invoice.AmountPaid;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Update billing profile totals
            await UpdateBillingProfileAsync(invoice.StudentBillingProfileId);

            return invoice;
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            // Update billing profile
            await UpdateBillingProfileAsync(invoice.StudentBillingProfileId);
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.Now.Year;
            var lastInvoice = await _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV-{year}"))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (int.TryParse(parts[2], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"INV-{year}-{nextNumber.ToString("D5")}";
        }

        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .Include(i => i.StudentBillingProfile)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task UpdateInvoiceStatusAsync(int invoiceId, string status)
        {
            var invoice = await GetInvoiceByIdAsync(invoiceId);
            if (invoice != null)
            {
                invoice.Status = status;
                if (status == "Paid")
                {
                    invoice.PaidDate = DateTime.Now;
                    invoice.AmountDue = 0;
                }
                await UpdateInvoiceAsync(invoice);
            }
        }

        private async Task UpdateBillingProfileAsync(int billingProfileId)
        {
            var profile = await _context.StudentBillingProfiles.FindAsync(billingProfileId);
            if (profile != null)
            {
                var invoices = await GetInvoicesByStudentAsync(billingProfileId);
                profile.TotalOutstanding = invoices.Where(i => i.Status != "Paid").Sum(i => i.AmountDue);
                profile.TotalPaid = invoices.Where(i => i.Status == "Paid").Sum(i => i.AmountPaid);

                _context.StudentBillingProfiles.Update(profile);
                await _context.SaveChangesAsync();
            }
        }
    }
}
