using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    public partial class initialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    admin_pass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_superadmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "Payrate",
                columns: table => new
                {
                    payrate_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    payrate_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payrate_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salary_multiplier = table.Column<float>(type: "real", nullable: false),
                    hourly_multiplier = table.Column<float>(type: "real", nullable: false),
                    flat_increase = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrate", x => x.payrate_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    role_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Training",
                columns: table => new
                {
                    training_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    training_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training", x => x.training_id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    current_admin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.company_id);
                    table.ForeignKey(
                        name: "FK_Company_Admin_admin_id",
                        column: x => x.admin_id,
                        principalTable: "Admin",
                        principalColumn: "admin_id");
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    shift_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    shift_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    shift_end = table.Column<DateTime>(type: "datetime2", nullable: false),
                    overtime_min = table.Column<int>(type: "int", nullable: false),
                    qr_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payrate_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    overtime_rate_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.shift_id);
                    table.ForeignKey(
                        name: "FK_Shift_Payrate_overtime_rate_id",
                        column: x => x.overtime_rate_id,
                        principalTable: "Payrate",
                        principalColumn: "payrate_id");
                    table.ForeignKey(
                        name: "FK_Shift_Payrate_payrate_id",
                        column: x => x.payrate_id,
                        principalTable: "Payrate",
                        principalColumn: "payrate_id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDetails",
                columns: table => new
                {
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    employee_id_by_company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employee_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parent_company = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_role = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    acc_pass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employer_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    employer_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employment_start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    year_of_services = table.Column<int>(type: "int", nullable: false),
                    types_of_wages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wages_rate = table.Column<float>(type: "real", nullable: false),
                    employement_letter = table.Column<bool>(type: "bit", nullable: false),
                    monthly_deduction = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDetails", x => x.employee_id);
                    table.ForeignKey(
                        name: "FK_EmployeeDetails_Company_parent_company",
                        column: x => x.parent_company,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDetails_EmployeeDetails_employer_id",
                        column: x => x.employer_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_EmployeeDetails_Role_staff_role",
                        column: x => x.staff_role,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    attendance_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    shift_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    validity = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.attendance_id);
                    table.ForeignKey(
                        name: "FK_Attendance_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Shift_shift_id",
                        column: x => x.shift_id,
                        principalTable: "Shift",
                        principalColumn: "shift_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Benefit",
                columns: table => new
                {
                    benefit_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    benefit_desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    benefit_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefit", x => x.benefit_id);
                    table.ForeignKey(
                        name: "FK_Benefit_EmployeeDetails_user_id",
                        column: x => x.user_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compensation",
                columns: table => new
                {
                    comp_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    comp_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comp_desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_applied = table.Column<DateTime>(type: "datetime2", nullable: false),
                    approved_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reject_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_completed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compensation", x => x.comp_id);
                    table.ForeignKey(
                        name: "FK_Compensation_EmployeeDetails_user_id",
                        column: x => x.user_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    document_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    owner_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    document_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notify_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_Document_EmployeeDetails_owner_id",
                        column: x => x.owner_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leave",
                columns: table => new
                {
                    leave_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    approval_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_by = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leave_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leave_end = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leave", x => x.leave_id);
                    table.ForeignKey(
                        name: "FK_Leave_EmployeeDetails_approved_by",
                        column: x => x.approved_by,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_Leave_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payroll",
                columns: table => new
                {
                    payroll_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    month_salary = table.Column<float>(type: "real", nullable: false),
                    overtime_pay = table.Column<float>(type: "real", nullable: false),
                    kwsp_total = table.Column<float>(type: "real", nullable: false),
                    zakat_total = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payroll", x => x.payroll_id);
                    table.ForeignKey(
                        name: "FK_Payroll_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    rating_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_rated = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    rated_by = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.rating_id);
                    table.ForeignKey(
                        name: "FK_Rating_EmployeeDetails_rated_by",
                        column: x => x.rated_by,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_Rating_EmployeeDetails_staff_rated",
                        column: x => x.staff_rated,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingProgress",
                columns: table => new
                {
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    training_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    completion = table.Column<bool>(type: "bit", nullable: false),
                    duration_left = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingProgress", x => new { x.staff_id, x.training_id });
                    table.ForeignKey(
                        name: "FK_TrainingProgress_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingProgress_Training_training_id",
                        column: x => x.training_id,
                        principalTable: "Training",
                        principalColumn: "training_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_shift_id",
                table: "Attendance",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_staff_id",
                table: "Attendance",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_Benefit_user_id",
                table: "Benefit",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Company_admin_id",
                table: "Company",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_Compensation_user_id",
                table: "Compensation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Document_owner_id",
                table: "Document",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_employer_id",
                table: "EmployeeDetails",
                column: "employer_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_parent_company",
                table: "EmployeeDetails",
                column: "parent_company");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_staff_role",
                table: "EmployeeDetails",
                column: "staff_role");

            migrationBuilder.CreateIndex(
                name: "IX_Leave_approved_by",
                table: "Leave",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_Leave_staff_id",
                table: "Leave",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_staff_id",
                table: "Payroll",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_rated_by",
                table: "Rating",
                column: "rated_by");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_staff_rated",
                table: "Rating",
                column: "staff_rated");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_overtime_rate_id",
                table: "Shift",
                column: "overtime_rate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_payrate_id",
                table: "Shift",
                column: "payrate_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgress_training_id",
                table: "TrainingProgress",
                column: "training_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "Benefit");

            migrationBuilder.DropTable(
                name: "Compensation");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Leave");

            migrationBuilder.DropTable(
                name: "Payroll");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "TrainingProgress");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "EmployeeDetails");

            migrationBuilder.DropTable(
                name: "Training");

            migrationBuilder.DropTable(
                name: "Payrate");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Admin");
        }
    }
}
