using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TagService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CountQuestion", "CountWotchers", "CreatedAt", "DailyRequestCount", "Description", "Name", "WeeklyRequestCount" },
                values: new object[,]
                {
                    { 1, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Язык C#", "csharp", 0 },
                    { 2, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "ASP.NET Core 9", "asp.net-core", 0 },
                    { 3, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Entity Framework Core", "entity-framework", 0 },
                    { 4, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "LINQ‑выражения", "linq", 0 },
                    { 5, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "SQL‑запросы", "sql", 0 },
                    { 6, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "PostgreSQL", "postgresql", 0 },
                    { 7, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Контейнеризация", "docker", 0 },
                    { 8, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Очереди RabbitMQ", "rabbitmq", 0 },
                    { 9, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "REST‑API", "rest", 0 },
                    { 10, 0, 0, new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Микросервисная арх‑ра", "microservices", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
