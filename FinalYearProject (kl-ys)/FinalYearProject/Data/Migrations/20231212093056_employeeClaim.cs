using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class employeeClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    claim_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    approval_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claim_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_apply = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reject_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claimAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claimFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.claim_id);
                    table.ForeignKey(
                        name: "FK_Claim_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_staff_id",
                table: "Claim",
                column: "staff_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");
        }
    }
}
