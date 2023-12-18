using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claim_EmployeeDetails_staff_id",
                table: "Claim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Claim",
                table: "Claim");

            migrationBuilder.RenameTable(
                name: "Claim",
                newName: "EmployeeClaim");

            migrationBuilder.RenameIndex(
                name: "IX_Claim_staff_id",
                table: "EmployeeClaim",
                newName: "IX_EmployeeClaim_staff_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeClaim",
                table: "EmployeeClaim",
                column: "claim_id");

            migrationBuilder.CreateTable(
                name: "Chatboxs",
                columns: table => new
                {
                    chat_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    admin_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    send_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    send_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    receive_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receive_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chat_ctn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    send_userid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatboxs", x => x.chat_id);
                    table.ForeignKey(
                        name: "FK_Chatboxs_Admin_admin_id",
                        column: x => x.admin_id,
                        principalTable: "Admin",
                        principalColumn: "admin_id");
                    table.ForeignKey(
                        name: "FK_Chatboxs_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTasks",
                columns: table => new
                {
                    emtask_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    emtask_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    current_admin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    progress_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_upload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    emtask_duration = table.Column<int>(type: "int", nullable: true),
                    emtaskDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    emtaskdoneFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTasks", x => x.emtask_id);
                    table.ForeignKey(
                        name: "FK_EmployeeTasks_Admin_current_admin",
                        column: x => x.current_admin,
                        principalTable: "Admin",
                        principalColumn: "admin_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeTasks_EmployeeDetails_staff_id",
                        column: x => x.staff_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    chatmsg_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    chat_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    send_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    send_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    receive_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receive_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chat_ctn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    send_userid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.chatmsg_id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Chatboxs_chat_id",
                        column: x => x.chat_id,
                        principalTable: "Chatboxs",
                        principalColumn: "chat_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatboxs_admin_id",
                table: "Chatboxs",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chatboxs_staff_id",
                table: "Chatboxs",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_chat_id",
                table: "ChatMessages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTasks_current_admin",
                table: "EmployeeTasks",
                column: "current_admin");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTasks_staff_id",
                table: "EmployeeTasks",
                column: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeClaim_EmployeeDetails_staff_id",
                table: "EmployeeClaim",
                column: "staff_id",
                principalTable: "EmployeeDetails",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeClaim_EmployeeDetails_staff_id",
                table: "EmployeeClaim");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "EmployeeTasks");

            migrationBuilder.DropTable(
                name: "Chatboxs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeClaim",
                table: "EmployeeClaim");

            migrationBuilder.RenameTable(
                name: "EmployeeClaim",
                newName: "Claim");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeClaim_staff_id",
                table: "Claim",
                newName: "IX_Claim_staff_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claim",
                table: "Claim",
                column: "claim_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_EmployeeDetails_staff_id",
                table: "Claim",
                column: "staff_id",
                principalTable: "EmployeeDetails",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
