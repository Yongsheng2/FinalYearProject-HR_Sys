using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixpayback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkInValid",
                table: "Attendance");

            migrationBuilder.RenameColumn(
                name: "checkOutValid",
                table: "Attendance",
                newName: "claimed");

            migrationBuilder.AddColumn<string>(
                name: "advance_id",
                table: "Payroll",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payback_id",
                table: "Payroll",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_advance_id",
                table: "Payroll",
                column: "advance_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_payback_id",
                table: "Payroll",
                column: "payback_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payroll_PayBack_payback_id",
                table: "Payroll",
                column: "payback_id",
                principalTable: "PayBack",
                principalColumn: "payback_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payroll_SalaryAdvance_advance_id",
                table: "Payroll",
                column: "advance_id",
                principalTable: "SalaryAdvance",
                principalColumn: "advance_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payroll_PayBack_payback_id",
                table: "Payroll");

            migrationBuilder.DropForeignKey(
                name: "FK_Payroll_SalaryAdvance_advance_id",
                table: "Payroll");

            migrationBuilder.DropIndex(
                name: "IX_Payroll_advance_id",
                table: "Payroll");

            migrationBuilder.DropIndex(
                name: "IX_Payroll_payback_id",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "advance_id",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "payback_id",
                table: "Payroll");

            migrationBuilder.RenameColumn(
                name: "claimed",
                table: "Attendance",
                newName: "checkOutValid");

            migrationBuilder.AddColumn<bool>(
                name: "checkInValid",
                table: "Attendance",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
