using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class permissionsAgain1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_AspNetRoles_identityRole",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_Permission_identityRole",
                table: "Permission");

            migrationBuilder.AlterColumn<string>(
                name: "identityRole",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "identityRole",
                table: "Permission",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_identityRole",
                table: "Permission",
                column: "identityRole");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_AspNetRoles_identityRole",
                table: "Permission",
                column: "identityRole",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
