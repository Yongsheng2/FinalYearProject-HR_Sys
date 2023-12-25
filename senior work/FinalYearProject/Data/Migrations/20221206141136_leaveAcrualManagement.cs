using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class leaveAcrualManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "leaveHoursPerDay",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "paidLeaveYearly",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "paidMaxCarryover",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sickLeaveYearly",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sickMaxCarryover",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "leaveHoursPerDay",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "paidLeaveYearly",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "paidMaxCarryover",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "sickLeaveYearly",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "sickMaxCarryover",
                table: "Company");
        }
    }
}
