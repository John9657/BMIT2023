using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class RefundService : IRefundService
    {
        private readonly AppDbContext _context;

        public RefundService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Refund>> GetAllRefundsAsync()
        {
            return await _context.Refunds
                .Include(r => r.Payment)
                .Include(r => r.StudentBillingProfile)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task<Refund> GetRefundByIdAsync(int id)
        {
            return await _context.Refunds
                .Include(r => r.Payment)
                .Include(r => r.StudentBillingProfile)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Refund>> GetRefundsByStudentAsync(int billingProfileId)
        {
            return await _context.Refunds
                .Where(r => r.StudentBillingProfileId == billingProfileId)
                .Include(r => r.Payment)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task<List<Refund>> GetPendingRefundsAsync()
        {
            return await _context.Refunds
                .Where(r => r.Status == "Pending")
                .Include(r => r.StudentBillingProfile)
                .OrderBy(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task<List<Refund>> GetApprovedRefundsAsync()
        {
            return await _context.Refunds
                .Where(r => r.Status == "Approved" || r.Status == "Processing")
                .Include(r => r.StudentBillingProfile)
                .OrderBy(r => r.ApprovedDate)
                .ToListAsync();
        }

        public async Task<Refund> CreateRefundAsync(Refund refund)
        {
            refund.RefundReference = await GenerateRefundReferenceAsync();
            refund.RequestedDate = DateTime.Now;
            refund.Status = "Pending";

            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            return refund;
        }

        public async Task UpdateRefundAsync(Refund refund)
        {
            _context.Refunds.Update(refund);
            await _context.SaveChangesAsync();
        }

        public async Task ApproveRefundAsync(int refundId, int adminId, string notes = "")
        {
            var refund = await GetRefundByIdAsync(refundId);
            if (refund != null)
            {
                refund.Status = "Approved";
                refund.ApprovedDate = DateTime.Now;
                refund.ApprovedByAdminId = adminId;
                refund.ApprovalNotes = notes;

                _context.Refunds.Update(refund);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectRefundAsync(int refundId, string reason)
        {
            var refund = await GetRefundByIdAsync(refundId);
            if (refund != null)
            {
                refund.Status = "Rejected";
                refund.RejectionReason = reason;

                _context.Refunds.Update(refund);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateRefundReferenceAsync()
        {
            var year = DateTime.Now.Year;
            var lastRefund = await _context.Refunds
                .Where(r => r.RefundReference.StartsWith($"REF-{year}"))
                .OrderByDescending(r => r.RefundReference)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastRefund != null)
            {
                var parts = lastRefund.RefundReference.Split('-');
                if (int.TryParse(parts[2], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"REF-{year}-{nextNumber.ToString("D5")}";
        }

        public async Task ProcessRefundAsync(int refundId)
        {
            var refund = await GetRefundByIdAsync(refundId);
            if (refund != null && refund.Status == "Approved")
            {
                refund.Status = "Processing";
                refund.CompletedDate = DateTime.Now;

                // Update student billing profile
                var profile = await _context.StudentBillingProfiles.FindAsync(refund.StudentBillingProfileId);
                if (profile != null)
                {
                    if (refund.RefundMethod == "Credit")
                    {
                        profile.CreditBalance += refund.RefundAmount;
                    }
                    else if (refund.RefundMethod == "Original" && refund.Payment != null)
                    {
                        profile.TotalPaid -= refund.RefundAmount;
                    }

                    _context.StudentBillingProfiles.Update(profile);
                }

                _context.Refunds.Update(refund);
                await _context.SaveChangesAsync();
            }
        }
    }
}
