using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class leaveAccrual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "paidLeaveHourBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "paidLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sickLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sickLeaveOnBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paidLeaveHourBargain",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "paidLeaveHourLeft",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "sickLeaveHourLeft",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "sickLeaveOnBargain",
                table: "EmployeeDetails");
        }
    }
}
