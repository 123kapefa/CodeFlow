using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbRefactorUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserStatisticId1",
                table: "UsersInfos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfos_UserStatisticId1",
                table: "UsersInfos",
                column: "UserStatisticId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInfos_UsersStatistic_UserStatisticId1",
                table: "UsersInfos",
                column: "UserStatisticId1",
                principalTable: "UsersStatistic",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersInfos_UsersStatistic_UserStatisticId1",
                table: "UsersInfos");

            migrationBuilder.DropIndex(
                name: "IX_UsersInfos_UserStatisticId1",
                table: "UsersInfos");

            migrationBuilder.DropColumn(
                name: "UserStatisticId1",
                table: "UsersInfos");
        }
    }
}
