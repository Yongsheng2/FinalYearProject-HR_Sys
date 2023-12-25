using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class serverFixEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "asp_id",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bank_name",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bank_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dob",
                table: "EmployeeDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "epf_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ic_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "itax_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profileImg_path",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sosco_no",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asp_id",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "bank_name",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "bank_no",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "dob",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "epf_no",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "ic_no",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "itax_no",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "phone_no",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "profileImg_path",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "sosco_no",
                table: "EmployeeDetails");
        }
    }
}
