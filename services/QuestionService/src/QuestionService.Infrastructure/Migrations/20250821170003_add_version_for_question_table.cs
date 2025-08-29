using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_version_for_question_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcceptedAnswerVersion",
                table: "Questions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAnswerVersion",
                table: "Questions");
        }
    }
}
