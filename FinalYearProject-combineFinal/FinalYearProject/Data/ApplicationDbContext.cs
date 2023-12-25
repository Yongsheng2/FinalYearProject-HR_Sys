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

            builder.Entity<SurveyQuestion>().HasKey(table => new {
                table.question_id,
                table.survey_id
            });

            builder.Entity<EmployeeIncentives>().HasKey(table => new {
                table.employee_id,
                table.incentives_id,
                table.start_Claimed,
                table.end_Claimed
            });

            builder.Entity<EmployeeKPI>().HasKey(table => new {
                table.employee_id,
                table.KPI_id
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
        public DbSet<Survey> Survey { get; set; }
        public DbSet<SurveyTaken> SurveyTaken { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestion { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }

        public DbSet<Incentives> Incentives { get; set; }

        public DbSet<CompanyKPI> CompanyKPIs { get; set; }

        public DbSet<EmployeeIncentives> EmployeeIncentives { get; set; }

        public DbSet<EmployeeKPI> EmployeeKPI { get; set; }

        public DbSet<SalaryAdvance> SalaryAdvance { get; set; }

        public DbSet<PayBack> PayBack { get; set; }

        public DbSet<EmployeeClaim> EmployeeClaim { get; set; }

        public DbSet<EmployeeTask> EmployeeTasks { get; set; }
        public DbSet<Chatbox> Chatboxs { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

    }
}