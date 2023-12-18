using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Survey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    answer_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    surveyTaken_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    question_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    answer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.answer_id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    question_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.question_id);
                });

            migrationBuilder.CreateTable(
                name: "Survey",
                columns: table => new
                {
                    survey_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Survey", x => x.survey_id);
                    table.ForeignKey(
                        name: "FK_Survey_EmployeeDetails_company_id",
                        column: x => x.company_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyQuestion",
                columns: table => new
                {
                    survey_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    question_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyQuestion", x => new { x.question_id, x.survey_id });
                    table.ForeignKey(
                        name: "FK_SurveyQuestion_Question_question_id",
                        column: x => x.question_id,
                        principalTable: "Question",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyQuestion_Survey_survey_id",
                        column: x => x.survey_id,
                        principalTable: "Survey",
                        principalColumn: "survey_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyTaken",
                columns: table => new
                {
                    surveyTaken_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    survey_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyTaken", x => x.surveyTaken_id);
                    table.ForeignKey(
                        name: "FK_SurveyTaken_EmployeeDetails_employee_id",
                        column: x => x.employee_id,
                        principalTable: "EmployeeDetails",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_SurveyTaken_Survey_survey_id",
                        column: x => x.survey_id,
                        principalTable: "Survey",
                        principalColumn: "survey_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Survey_company_id",
                table: "Survey",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestion_survey_id",
                table: "SurveyQuestion",
                column: "survey_id");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyTaken_employee_id",
                table: "SurveyTaken",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyTaken_survey_id",
                table: "SurveyTaken",
                column: "survey_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "SurveyQuestion");

            migrationBuilder.DropTable(
                name: "SurveyTaken");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Survey");
        }
    }
}
