using FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinalYearProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TrainingProgress>().HasKey(table => new {
                table.staff_id,
                table.training_id
            });

        }

        public DbSet<Admin> Admin { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Payrate> Payrate { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Training> Training { get; set; }
        public DbSet<TrainingProgress> TrainingProgress { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Benefit> Benefit { get; set; }
        public DbSet<Compensation> Compensation { get; set; }
        public DbSet<Leave> Leave { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<Permission> Permission { get; set; }
    }
}