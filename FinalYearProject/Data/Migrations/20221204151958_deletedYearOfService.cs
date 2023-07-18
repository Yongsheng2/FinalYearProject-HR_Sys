using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    public partial class deletedYearOfService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "year_of_services",
                table: "EmployeeDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "year_of_services",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
