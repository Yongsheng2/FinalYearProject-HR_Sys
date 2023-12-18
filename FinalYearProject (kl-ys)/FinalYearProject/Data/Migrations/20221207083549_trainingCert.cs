using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class trainingCert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cert_id",
                table: "TrainingProgress",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgress_cert_id",
                table: "TrainingProgress",
                column: "cert_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingProgress_Document_cert_id",
                table: "TrainingProgress",
                column: "cert_id",
                principalTable: "Document",
                principalColumn: "document_id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingProgress_Document_cert_id",
                table: "TrainingProgress");

            migrationBuilder.DropIndex(
                name: "IX_TrainingProgress_cert_id",
                table: "TrainingProgress");

            migrationBuilder.DropColumn(
                name: "cert_id",
                table: "TrainingProgress");
        }
    }
}
