using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class salaryAdvance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Statisfy_the_Company",
                table: "SurveyTaken",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_score",
                table: "SurveyTaken",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "survey_name",
                table: "Survey",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "rate_date",
                table: "Rating",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "questionType_id",
                table: "Question",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Payroll",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "incentives_total",
                table: "Payroll",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Payroll",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "answer",
                table: "Answer",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyKPIs",
                columns: table => new
                {
                    KPI_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    target_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    target_KPI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    hit_KPI_allowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyKPIs", x => x.KPI_id);
                    table.ForeignKey(
                        name: "FK_CompanyKPIs_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeIncentives",
                columns: table => new
                {
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    incentives_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    start_Claimed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_Claimed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeIncentives", x => new { x.employee_id, x.incentives_id, x.start_Claimed, x.end_Claimed });
                });

            migrationBuilder.CreateTable(
                name: "EmployeeKPI",
                columns: table => new
                {
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KPI_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeKPI", x => new { x.employee_id, x.KPI_id });
                });

            migrationBuilder.CreateTable(
                name: "Incentives",
                columns: table => new
                {
                    incentives_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    good_rating_incentives = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    attendance_Excellence_Award = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incentives", x => x.incentives_id);
                    table.ForeignKey(
                        name: "FK_Incentives_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionType",
                columns: table => new
                {
                    questionType_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    questionType_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionType", x => x.questionType_id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryAdvance",
                columns: table => new
                {
                    advance_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<float>(type: "real", nullable: true),
                    time_to_payback = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    request_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    approved_by = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryAdvance", x => x.advance_id);
                    table.ForeignKey(
                        name: "FK_SalaryAdvance_EmployeeDetails_approved_by",
                        column: x => x.approved_by,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_SalaryAdvance_EmployeeDetails_employee_id",
                        column: x => x.employee_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                });

            migrationBuilder.CreateTable(
                name: "PayBack",
                columns: table => new
                {
                    payback_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    advance_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    payback_amount = table.Column<float>(type: "real", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayBack", x => x.payback_id);
                    table.ForeignKey(
                        name: "FK_PayBack_SalaryAdvance_advance_id",
                        column: x => x.advance_id,
                        principalTable: "SalaryAdvance",
                        principalColumn: "advance_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_questionType_id",
                table: "Question",
                column: "questionType_id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyKPIs_company_id",
                table: "CompanyKPIs",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_Incentives_company_id",
                table: "Incentives",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayBack_advance_id",
                table: "PayBack",
                column: "advance_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAdvance_approved_by",
                table: "SalaryAdvance",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAdvance_employee_id",
                table: "SalaryAdvance",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_QuestionType_questionType_id",
                table: "Question",
                column: "questionType_id",
                principalTable: "QuestionType",
                principalColumn: "questionType_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_QuestionType_questionType_id",
                table: "Question");

            migrationBuilder.DropTable(
                name: "CompanyKPIs");

            migrationBuilder.DropTable(
                name: "EmployeeIncentives");

            migrationBuilder.DropTable(
                name: "EmployeeKPI");

            migrationBuilder.DropTable(
                name: "Incentives");

            migrationBuilder.DropTable(
                name: "PayBack");

            migrationBuilder.DropTable(
                name: "QuestionType");

            migrationBuilder.DropTable(
                name: "SalaryAdvance");

            migrationBuilder.DropIndex(
                name: "IX_Question_questionType_id",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "Statisfy_the_Company",
                table: "SurveyTaken");

            migrationBuilder.DropColumn(
                name: "total_score",
                table: "SurveyTaken");

            migrationBuilder.DropColumn(
                name: "survey_name",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "rate_date",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "questionType_id",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "incentives_total",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Payroll");

            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "Answer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
