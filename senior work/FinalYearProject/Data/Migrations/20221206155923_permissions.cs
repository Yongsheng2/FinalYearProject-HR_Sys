using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "company_id",
                table: "Role",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_company_id",
                table: "Role",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Company_company_id",
                table: "Role",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Company_company_id",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_company_id",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "Role");
        }
    }
}
