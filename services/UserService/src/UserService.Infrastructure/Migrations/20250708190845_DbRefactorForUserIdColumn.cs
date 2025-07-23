using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbRefactorForUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersStatistic_UsersInfos_UserId",
                table: "UsersStatistic");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfos_UserStatisticId",
                table: "UsersInfos",
                column: "UserStatisticId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInfos_UsersStatistic_UserStatisticId",
                table: "UsersInfos",
                column: "UserStatisticId",
                principalTable: "UsersStatistic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersInfos_UsersStatistic_UserStatisticId",
                table: "UsersInfos");

            migrationBuilder.DropIndex(
                name: "IX_UsersInfos_UserStatisticId",
                table: "UsersInfos");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersStatistic_UsersInfos_UserId",
                table: "UsersStatistic",
                column: "UserId",
                principalTable: "UsersInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
