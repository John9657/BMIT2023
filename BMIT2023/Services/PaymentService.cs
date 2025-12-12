using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.StudentBillingProfile)
                .Include(p => p.Invoice)
                .Include(p => p.PaymentGateway)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.StudentBillingProfile)
                .Include(p => p.Invoice)
                .Include(p => p.PaymentGateway)
                .Include(p => p.PaymentTrackings)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetPaymentsByStudentAsync(int billingProfileId)
        {
            return await _context.Payments
                .Where(p => p.StudentBillingProfileId == billingProfileId)
                .Include(p => p.Invoice)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .Include(p => p.StudentBillingProfile)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "Pending" || p.Status == "Processing")
                .Include(p => p.StudentBillingProfile)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            payment.PaymentReference = await GeneratePaymentReferenceAsync();
            payment.PaymentDate = DateTime.Now;
            payment.Status = "Pending";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Create initial payment tracking
            var tracking = new PaymentTracking
            {
                PaymentId = payment.Id,
                Status = "Initiated",
                StatusChangedDate = DateTime.Now,
                AmountTracked = payment.Amount,
                Details = "Payment initiated"
            };
            _context.PaymentTrackings.Add(tracking);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GeneratePaymentReferenceAsync()
        {
            var year = DateTime.Now.Year;
            var lastPayment = await _context.Payments
                .Where(p => p.PaymentReference.StartsWith($"PAY-{year}"))
                .OrderByDescending(p => p.PaymentReference)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastPayment != null)
            {
                var parts = lastPayment.PaymentReference.Split('-');
                if (int.TryParse(parts[2], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"PAY-{year}-{nextNumber.ToString("D5")}";
        }

        public async Task ProcessPaymentAsync(int paymentId)
        {
            var payment = await GetPaymentByIdAsync(paymentId);
            if (payment != null)
            {
                payment.Status = "Processing";
                payment.ProcessedDate = DateTime.Now;

                // Add tracking record
                var tracking = new PaymentTracking
                {
                    PaymentId = paymentId,
                    Status = "Authorized",
                    StatusChangedDate = DateTime.Now,
                    AmountTracked = payment.Amount,
                    Details = "Payment authorized by gateway"
                };
                _context.PaymentTrackings.Add(tracking);

                // Update invoice if linked
                if (payment.InvoiceId.HasValue)
                {
                    var invoice = await _context.Invoices.FindAsync(payment.InvoiceId.Value);
                    if (invoice != null)
                    {
                        invoice.AmountPaid += payment.Amount;
                        invoice.AmountDue -= payment.Amount;

                        if (invoice.AmountDue <= 0)
                        {
                            invoice.Status = "Paid";
                            invoice.PaidDate = DateTime.Now;
                        }
                        else if (invoice.AmountPaid > 0)
                        {
                            invoice.Status = "PartiallyPaid";
                        }
                        _context.Invoices.Update(invoice);
                    }
                }

                // Update billing profile
                var profile = await _context.StudentBillingProfiles.FindAsync(payment.StudentBillingProfileId);
                if (profile != null)
                {
                    profile.TotalPaid += payment.Amount;
                    profile.TotalOutstanding -= payment.Amount;
                    profile.LastPaymentDate = DateTime.Now;
                    _context.StudentBillingProfiles.Update(profile);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateGatewayFeesAsync(decimal amount, int gatewayId)
        {
            var gateway = await _context.PaymentGateways.FindAsync(gatewayId);
            if (gateway != null)
            {
                decimal percentageFee = (amount * gateway.TransactionFeePercent) / 100;
                return percentageFee + gateway.TransactionFeeFixed;
            }
            return 0;
        }

        public async Task<List<Payment>> GetUnreconciledPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => !p.IsReconciled && p.Status == "Successful")
                .Include(p => p.StudentBillingProfile)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
