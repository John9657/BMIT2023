using BMIT2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BMIT2023.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- SEED DATA for Admin and Teacher ---

            // Admin Data
            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 1, AdminId = "A001", FullName = "System Admin", Email = "admin@bmit.com", Password = "password123" }
            );

            // Teacher Data
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, TeacherId = "T001", FullName = "Dr. Alice Smith", Email = "alice.smith@bmit.edu", Password = "teacherpass" },
                new Teacher { Id = 2, TeacherId = "T002", FullName = "Mr. Bob Johnson", Email = "bob.j@bmit.edu", Password = "teacherpass" }
            );

            modelBuilder.Entity<Student>().HasIndex(s => s.StudentId).IsUnique();
            modelBuilder.Entity<Teacher>().HasIndex(t => t.TeacherId).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(a => a.AdminId).IsUnique();
        }
    }
}