using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class integration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "leave_id",
                table: "Attendance",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_leave_id",
                table: "Attendance",
                column: "leave_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Leave_leave_id",
                table: "Attendance",
                column: "leave_id",
                principalTable: "Leave",
                principalColumn: "leave_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Leave_leave_id",
                table: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_leave_id",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "leave_id",
                table: "Attendance");
        }
    }
}
