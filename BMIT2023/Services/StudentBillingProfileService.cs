using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class StudentBillingProfileService : IStudentBillingProfileService
    {
        private readonly AppDbContext _context;

        public StudentBillingProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentBillingProfile> GetProfileByIdAsync(int id)
        {
            return await _context.StudentBillingProfiles
                .Include(p => p.Student)
                .Include(p => p.Invoices)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<StudentBillingProfile> GetProfileByStudentIdAsync(int studentId)
        {
            return await _context.StudentBillingProfiles
                .Include(p => p.Student)
                .Include(p => p.Invoices)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.StudentId == studentId);
        }

        public async Task<StudentBillingProfile> CreateProfileAsync(StudentBillingProfile profile)
        {
            profile.CreatedDate = DateTime.Now;
            profile.BillingStatus = "Active";

            _context.StudentBillingProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task UpdateProfileAsync(StudentBillingProfile profile)
        {
            _context.StudentBillingProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StudentBillingProfile>> GetAllProfilesAsync()
        {
            return await _context.StudentBillingProfiles
                .Include(p => p.Student)
                .OrderBy(p => p.Student.FullName)
                .ToListAsync();
        }

        public async Task<List<StudentBillingProfile>> GetProfilesByStatusAsync(string status)
        {
            return await _context.StudentBillingProfiles
                .Where(p => p.BillingStatus == status)
                .Include(p => p.Student)
                .OrderBy(p => p.Student.FullName)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalOutstandingAsync(int profileId)
        {
            var profile = await GetProfileByIdAsync(profileId);
            return profile?.TotalOutstanding ?? 0;
        }

        public async Task<decimal> GetTotalPaidAsync(int profileId)
        {
            var profile = await GetProfileByIdAsync(profileId);
            return profile?.TotalPaid ?? 0;
        }

        public async Task UpdateProfileStatusAsync(int profileId, string status)
        {
            var profile = await GetProfileByIdAsync(profileId);
            if (profile != null)
            {
                profile.BillingStatus = status;
                await UpdateProfileAsync(profile);
            }
        }
    }
}
