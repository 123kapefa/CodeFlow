using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastVisitAt",
                table: "UsersStatistic",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserStatisticId",
                table: "UsersInfos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UsersStatistic_UserId",
                table: "UsersStatistic",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersStatistic_UsersInfos_UserId",
                table: "UsersStatistic",
                column: "UserId",
                principalTable: "UsersInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersStatistic_UsersInfos_UserId",
                table: "UsersStatistic");

            migrationBuilder.DropIndex(
                name: "IX_UsersStatistic_UserId",
                table: "UsersStatistic");

            migrationBuilder.DropColumn(
                name: "UserStatisticId",
                table: "UsersInfos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastVisitAt",
                table: "UsersStatistic",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
