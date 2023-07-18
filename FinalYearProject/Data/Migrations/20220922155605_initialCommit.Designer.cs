﻿// <auto-generated />
using System;
using FinalYearProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220922155605_initialCommit")]
    partial class initialCommit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FinalYearProject.Models.Admin", b =>
                {
                    b.Property<string>("admin_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("admin_pass")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("is_superadmin")
                        .HasColumnType("bit");

                    b.HasKey("admin_id");

                    b.ToTable("Admin");
                });

            modelBuilder.Entity("FinalYearProject.Models.Attendance", b =>
                {
                    b.Property<string>("attendance_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("end_time")
                        .HasColumnType("datetime2");

                    b.Property<string>("shift_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("staff_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("start_time")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<bool>("validity")
                        .HasColumnType("bit");

                    b.HasKey("attendance_id");

                    b.HasIndex("shift_id");

                    b.HasIndex("staff_id");

                    b.ToTable("Attendance");
                });

            modelBuilder.Entity("FinalYearProject.Models.Benefit", b =>
                {
                    b.Property<string>("benefit_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("benefit_desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("benefit_type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("days")
                        .HasColumnType("int");

                    b.Property<DateTime?>("end_date")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("start_date")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("user_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("benefit_id");

                    b.HasIndex("user_id");

                    b.ToTable("Benefit");
                });

            modelBuilder.Entity("FinalYearProject.Models.Company", b =>
                {
                    b.Property<string>("company_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("admin_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("company_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("current_admin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.HasKey("company_id");

                    b.HasIndex("admin_id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("FinalYearProject.Models.Compensation", b =>
                {
                    b.Property<int>("comp_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("comp_id"), 1L, 1);

                    b.Property<string>("approved_by")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("comp_desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("comp_type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("date_applied")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("date_completed")
                        .HasColumnType("datetime2");

                    b.Property<string>("reject_reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("user_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("comp_id");

                    b.HasIndex("user_id");

                    b.ToTable("Compensation");
                });

            modelBuilder.Entity("FinalYearProject.Models.Document", b =>
                {
                    b.Property<string>("document_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.Property<string>("document_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("expiry_date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("notify_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("owner_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("document_id");

                    b.HasIndex("owner_id");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("FinalYearProject.Models.EmployeeDetails", b =>
                {
                    b.Property<string>("employee_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("acc_pass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("employee_id_by_company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("employee_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("employement_letter")
                        .HasColumnType("bit");

                    b.Property<string>("employer_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("employer_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("employment_start_date")
                        .HasColumnType("datetime2");

                    b.Property<float>("monthly_deduction")
                        .HasColumnType("real");

                    b.Property<string>("parent_company")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("staff_role")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("types_of_wages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("user_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("wages_rate")
                        .HasColumnType("real");

                    b.Property<int>("year_of_services")
                        .HasColumnType("int");

                    b.HasKey("employee_id");

                    b.HasIndex("employer_id");

                    b.HasIndex("parent_company");

                    b.HasIndex("staff_role");

                    b.ToTable("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.Leave", b =>
                {
                    b.Property<string>("leave_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("approval_status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("approved_by")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("leave_end")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("leave_start")
                        .HasColumnType("datetime2");

                    b.Property<string>("staff_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("leave_id");

                    b.HasIndex("approved_by");

                    b.HasIndex("staff_id");

                    b.ToTable("Leave");
                });

            modelBuilder.Entity("FinalYearProject.Models.Payrate", b =>
                {
                    b.Property<string>("payrate_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("flat_increase")
                        .HasColumnType("real");

                    b.Property<float>("hourly_multiplier")
                        .HasColumnType("real");

                    b.Property<string>("payrate_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("payrate_type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("salary_multiplier")
                        .HasColumnType("real");

                    b.HasKey("payrate_id");

                    b.ToTable("Payrate");
                });

            modelBuilder.Entity("FinalYearProject.Models.Payroll", b =>
                {
                    b.Property<string>("payroll_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("kwsp_total")
                        .HasColumnType("real");

                    b.Property<float>("month_salary")
                        .HasColumnType("real");

                    b.Property<float>("overtime_pay")
                        .HasColumnType("real");

                    b.Property<string>("staff_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("zakat_total")
                        .HasColumnType("real");

                    b.HasKey("payroll_id");

                    b.HasIndex("staff_id");

                    b.ToTable("Payroll");
                });

            modelBuilder.Entity("FinalYearProject.Models.Rating", b =>
                {
                    b.Property<string>("rating_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("rated_by")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("rating")
                        .HasColumnType("int");

                    b.Property<string>("staff_rated")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("rating_id");

                    b.HasIndex("rated_by");

                    b.HasIndex("staff_rated");

                    b.ToTable("Rating");
                });

            modelBuilder.Entity("FinalYearProject.Models.Role", b =>
                {
                    b.Property<string>("role_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("date_created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("role_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("role_id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("FinalYearProject.Models.Shift", b =>
                {
                    b.Property<string>("shift_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("overtime_min")
                        .HasColumnType("int");

                    b.Property<string>("overtime_rate_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("payrate_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("qr_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("shift_end")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("shift_start")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.HasKey("shift_id");

                    b.HasIndex("overtime_rate_id");

                    b.HasIndex("payrate_id");

                    b.ToTable("Shift");
                });

            modelBuilder.Entity("FinalYearProject.Models.Training", b =>
                {
                    b.Property<string>("training_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("duration")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("start_date")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("training_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("training_id");

                    b.ToTable("Training");
                });

            modelBuilder.Entity("FinalYearProject.Models.TrainingProgress", b =>
                {
                    b.Property<string>("staff_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("training_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("completion")
                        .HasColumnType("bit");

                    b.Property<int>("duration_left")
                        .HasColumnType("int");

                    b.HasKey("staff_id", "training_id");

                    b.HasIndex("training_id");

                    b.ToTable("TrainingProgress");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("FinalYearProject.Models.Attendance", b =>
                {
                    b.HasOne("FinalYearProject.Models.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("shift_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("staff_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("FinalYearProject.Models.Benefit", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.Company", b =>
                {
                    b.HasOne("FinalYearProject.Models.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("admin_id");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FinalYearProject.Models.Compensation", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.Document", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("owner_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.EmployeeDetails", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "Employer")
                        .WithMany()
                        .HasForeignKey("employer_id");

                    b.HasOne("FinalYearProject.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("parent_company")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FinalYearProject.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("staff_role");

                    b.Navigation("Company");

                    b.Navigation("Employer");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("FinalYearProject.Models.Leave", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "ApprovedByEmployee")
                        .WithMany()
                        .HasForeignKey("approved_by");

                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("staff_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApprovedByEmployee");

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.Payroll", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("staff_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("FinalYearProject.Models.Rating", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "RateByEmployee")
                        .WithMany()
                        .HasForeignKey("rated_by");

                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeRated")
                        .WithMany()
                        .HasForeignKey("staff_rated")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeRated");

                    b.Navigation("RateByEmployee");
                });

            modelBuilder.Entity("FinalYearProject.Models.Shift", b =>
                {
                    b.HasOne("FinalYearProject.Models.Payrate", "OvertimeRate")
                        .WithMany()
                        .HasForeignKey("overtime_rate_id");

                    b.HasOne("FinalYearProject.Models.Payrate", "Payrate")
                        .WithMany()
                        .HasForeignKey("payrate_id");

                    b.Navigation("OvertimeRate");

                    b.Navigation("Payrate");
                });

            modelBuilder.Entity("FinalYearProject.Models.TrainingProgress", b =>
                {
                    b.HasOne("FinalYearProject.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("staff_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FinalYearProject.Models.Training", "Training")
                        .WithMany()
                        .HasForeignKey("training_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");

                    b.Navigation("Training");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
