using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTagParticipationQuestion_UserTagParticipations_UserTagP~",
                table: "UserTagParticipationQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTagParticipationQuestion",
                table: "UserTagParticipationQuestion");

            migrationBuilder.RenameTable(
                name: "UserTagParticipationQuestion",
                newName: "UserTagParticipationQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_UserTagParticipationQuestion_UserTagParticipationId",
                table: "UserTagParticipationQuestions",
                newName: "IX_UserTagParticipationQuestions_UserTagParticipationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTagParticipationQuestions",
                table: "UserTagParticipationQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTagParticipationQuestions_UserTagParticipations_UserTag~",
                table: "UserTagParticipationQuestions",
                column: "UserTagParticipationId",
                principalTable: "UserTagParticipations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTagParticipationQuestions_UserTagParticipations_UserTag~",
                table: "UserTagParticipationQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTagParticipationQuestions",
                table: "UserTagParticipationQuestions");

            migrationBuilder.RenameTable(
                name: "UserTagParticipationQuestions",
                newName: "UserTagParticipationQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_UserTagParticipationQuestions_UserTagParticipationId",
                table: "UserTagParticipationQuestion",
                newName: "IX_UserTagParticipationQuestion_UserTagParticipationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTagParticipationQuestion",
                table: "UserTagParticipationQuestion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTagParticipationQuestion_UserTagParticipations_UserTagP~",
                table: "UserTagParticipationQuestion",
                column: "UserTagParticipationId",
                principalTable: "UserTagParticipations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
