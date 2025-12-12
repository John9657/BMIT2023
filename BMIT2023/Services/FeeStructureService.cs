using BMIT2023.Data;
using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMIT2023.Services
{
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

    public class FeeStructureService : IFeeStructureService
    {
        private readonly AppDbContext _context;

        public FeeStructureService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FeeStructure>> GetAllFeesAsync()
        {
            return await _context.FeeStructures.ToListAsync();
        }

        public async Task<FeeStructure> GetFeeByIdAsync(int id)
        {
            return await _context.FeeStructures.FindAsync(id);
        }

        public async Task<List<FeeStructure>> GetActiveFeesAsync()
        {
            return await _context.FeeStructures
                .Where(f => f.IsActive && DateTime.Now >= f.EffectiveFrom && (f.EffectiveTo == null || DateTime.Now <= f.EffectiveTo))
                .ToListAsync();
        }

        public async Task<List<FeeStructure>> GetMandatoryFeesAsync()
        {
            return await _context.FeeStructures
                .Where(f => f.IsActive && f.IsMandatory)
                .ToListAsync();
        }

        public async Task CreateFeeAsync(FeeStructure fee)
        {
            fee.CreatedDate = DateTime.Now;
            _context.FeeStructures.Add(fee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFeeAsync(FeeStructure fee)
        {
            fee.ModifiedDate = DateTime.Now;
            _context.FeeStructures.Update(fee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFeeAsync(int id)
        {
            var fee = await _context.FeeStructures.FindAsync(id);
            if (fee != null)
            {
                _context.FeeStructures.Remove(fee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateTotalFeesForStudentAsync(int studentId)
        {
            var fees = await GetMandatoryFeesAsync();
            return fees.Sum(f => f.Amount);
        }
    }
}
