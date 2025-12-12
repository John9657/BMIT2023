using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class DunningService : IDunningService
    {
        private readonly AppDbContext _context;
        private const int Level1Days = 7;      // 7 days overdue
        private const int Level2Days = 15;     // 15 days overdue
        private const int Level3Days = 30;     // 30 days overdue
        private const int Level4Days = 60;     // 60 days overdue

        public DunningService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DunningNotice>> GetAllNoticesAsync()
        {
            return await _context.DunningNotices
                .Include(d => d.StudentBillingProfile)
                .Include(d => d.Invoice)
                .OrderByDescending(d => d.NoticeDate)
                .ToListAsync();
        }

        public async Task<DunningNotice> GetNoticeByIdAsync(int id)
        {
            return await _context.DunningNotices
                .Include(d => d.StudentBillingProfile)
                .Include(d => d.Invoice)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DunningNotice>> GetNoticesByStudentAsync(int billingProfileId)
        {
            return await _context.DunningNotices
                .Where(d => d.StudentBillingProfileId == billingProfileId)
                .Include(d => d.Invoice)
                .OrderByDescending(d => d.NoticeDate)
                .ToListAsync();
        }

        public async Task<List<DunningNotice>> GetPendingNoticesAsync()
        {
            return await _context.DunningNotices
                .Where(d => d.Status == "Sent" || d.Status == "Escalated")
                .Include(d => d.StudentBillingProfile)
                .OrderBy(d => d.NoticeTargetDate)
                .ToListAsync();
        }

        public async Task<List<DunningNotice>> GetOverdueNoticesAsync()
        {
            return await _context.DunningNotices
                .Where(d => d.NoticeTargetDate < DateTime.Now && d.Status != "Resolved")
                .Include(d => d.StudentBillingProfile)
                .OrderBy(d => d.NoticeTargetDate)
                .ToListAsync();
        }

        public async Task<DunningNotice> CreateDunningNoticeAsync(DunningNotice notice)
        {
            notice.NoticeNumber = await GenerateNoticeNumberAsync();
            notice.NoticeDate = DateTime.Now;
            notice.Status = "Sent";

            _context.DunningNotices.Add(notice);
            await _context.SaveChangesAsync();

            return notice;
        }

        public async Task UpdateNoticeAsync(DunningNotice notice)
        {
            _context.DunningNotices.Update(notice);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateNoticeNumberAsync()
        {
            var year = DateTime.Now.Year;
            var lastNotice = await _context.DunningNotices
                .Where(d => d.NoticeNumber.StartsWith($"DN-{year}"))
                .OrderByDescending(d => d.NoticeNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastNotice != null)
            {
                var parts = lastNotice.NoticeNumber.Split('-');
                if (int.TryParse(parts[2], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"DN-{year}-{nextNumber.ToString("D5")}";
        }

        public async Task GenerateAutomaticDunningNoticesAsync()
        {
            var overdueInvoices = await _context.Invoices
                .Where(i => i.Status != "Paid" && i.Status != "Cancelled" && DateTime.Now > i.DueDate)
                .Include(i => i.StudentBillingProfile)
                .ToListAsync();

            foreach (var invoice in overdueInvoices)
            {
                var daysOverdue = DateTime.Now.Subtract(invoice.DueDate).Days;
                string noticeLevel = GetNoticeLevel(daysOverdue);

                // Check if notice already exists for this invoice at this level
                var existingNotice = await _context.DunningNotices
                    .FirstOrDefaultAsync(d => d.InvoiceId == invoice.Id && d.NoticeLevel == noticeLevel && d.Status != "Resolved");

                if (existingNotice == null)
                {
                    var notice = new DunningNotice
                    {
                        StudentBillingProfileId = invoice.StudentBillingProfileId,
                        InvoiceId = invoice.Id,
                        NoticeLevel = noticeLevel,
                        OutstandingAmount = invoice.AmountDue,
                        DueDateOriginal = invoice.DueDate,
                        DaysOverdue = DateTime.Now,
                        NoticeDate = DateTime.Now,
                        NoticeTargetDate = DateTime.Now.AddDays(7), // 7 days to resolve
                        Status = "Sent",
                        IsAutomatic = true,
                        NotificationMethod = "Email"
                    };

                    await CreateDunningNoticeAsync(notice);
                }
            }
        }

        public async Task MarkNoticeAsResolvedAsync(int noticeId, string notes)
        {
            var notice = await GetNoticeByIdAsync(noticeId);
            if (notice != null)
            {
                notice.Status = "Resolved";
                notice.ResolutionDate = DateTime.Now;
                notice.ResolutionNotes = notes;

                await UpdateNoticeAsync(notice);
            }
        }

        public async Task EscalateNoticeAsync(int noticeId)
        {
            var notice = await GetNoticeByIdAsync(noticeId);
            if (notice != null)
            {
                notice.Status = "Escalated";
                notice.EscalationLevel = (notice.EscalationLevel ?? 0) + 1;
                notice.EscalatedToAdminDate = DateTime.Now;

                // Move to next notice level
                if (notice.NoticeLevel == "Level1")
                    notice.NoticeLevel = "Level2";
                else if (notice.NoticeLevel == "Level2")
                    notice.NoticeLevel = "Level3";
                else if (notice.NoticeLevel == "Level3")
                    notice.NoticeLevel = "Level4";

                await UpdateNoticeAsync(notice);
            }
        }

        public async Task<List<DunningNotice>> GetNoticesByLevelAsync(string level)
        {
            return await _context.DunningNotices
                .Where(d => d.NoticeLevel == level)
                .Include(d => d.StudentBillingProfile)
                .OrderByDescending(d => d.NoticeDate)
                .ToListAsync();
        }

        private string GetNoticeLevel(int daysOverdue)
        {
            if (daysOverdue >= Level4Days)
                return "Level4";
            else if (daysOverdue >= Level3Days)
                return "Level3";
            else if (daysOverdue >= Level2Days)
                return "Level2";
            else if (daysOverdue >= Level1Days)
                return "Level1";
            else
                return "Level1";
        }
    }
}
