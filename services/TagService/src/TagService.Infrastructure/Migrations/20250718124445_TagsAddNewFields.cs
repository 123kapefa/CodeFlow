using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TagsAddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DailyCountUpdatedAt",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WeeklyCountUpdatedAt",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DailyCountUpdatedAt", "WeeklyCountUpdatedAt" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyCountUpdatedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "WeeklyCountUpdatedAt",
                table: "Tags");
        }
    }
}
