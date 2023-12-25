using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class leaveAccrualPlus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paidLeaveHourBargain",
                table: "EmployeeDetails");

            migrationBuilder.AddColumn<string>(
                name: "leaveType",
                table: "Leave",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sickLeaveOnBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sickLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "paidLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "paidLeaveOnBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "leaveType",
                table: "Leave");

            migrationBuilder.DropColumn(
                name: "paidLeaveOnBargain",
                table: "EmployeeDetails");

            migrationBuilder.AlterColumn<int>(
                name: "sickLeaveOnBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "sickLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "paidLeaveHourLeft",
                table: "EmployeeDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "paidLeaveHourBargain",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);
        }
    }
}
