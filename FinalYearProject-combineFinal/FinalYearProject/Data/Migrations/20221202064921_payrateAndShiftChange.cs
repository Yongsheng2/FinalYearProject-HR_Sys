using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    public partial class payrateAndShiftChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Payrate_overtime_rate_id",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "overtime_min",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "flat_increase",
                table: "Payrate");

            migrationBuilder.DropColumn(
                name: "hourly_multiplier",
                table: "Payrate");

            migrationBuilder.DropColumn(
                name: "payrate_type",
                table: "Payrate");

            migrationBuilder.DropColumn(
                name: "salary_multiplier",
                table: "Payrate");

            migrationBuilder.RenameColumn(
                name: "overtime_rate_id",
                table: "Shift",
                newName: "parent_shift");

            migrationBuilder.RenameIndex(
                name: "IX_Shift_overtime_rate_id",
                table: "Shift",
                newName: "IX_Shift_parent_shift");

            migrationBuilder.AddColumn<bool>(
                name: "is_overtime",
                table: "Shift",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "payrate_ratePerHour",
                table: "Payrate",
                type: "real",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Shift_parent_shift",
                table: "Shift",
                column: "parent_shift",
                principalTable: "Shift",
                principalColumn: "shift_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Shift_parent_shift",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "is_overtime",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "payrate_ratePerHour",
                table: "Payrate");

            migrationBuilder.RenameColumn(
                name: "parent_shift",
                table: "Shift",
                newName: "overtime_rate_id");

            migrationBuilder.RenameIndex(
                name: "IX_Shift_parent_shift",
                table: "Shift",
                newName: "IX_Shift_overtime_rate_id");

            migrationBuilder.AddColumn<int>(
                name: "overtime_min",
                table: "Shift",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "flat_increase",
                table: "Payrate",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "hourly_multiplier",
                table: "Payrate",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "payrate_type",
                table: "Payrate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "salary_multiplier",
                table: "Payrate",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Payrate_overtime_rate_id",
                table: "Shift",
                column: "overtime_rate_id",
                principalTable: "Payrate",
                principalColumn: "payrate_id");
        }
    }
}
