using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AECPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedWithGuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<long>(
                name: "OutputTokens",
                table: "ConversationLogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "InputTokens",
                table: "ConversationLogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LaborBudget", "Name", "ProjectNumber", "SpentToDate", "Status", "TotalBudget" },
                values: new object[,]
                {
                    { new Guid("2d5e865a-9310-4828-9717-3f338d38827c"), new DateTime(2026, 4, 20, 11, 34, 38, 683, DateTimeKind.Utc).AddTicks(309), null, 0m, "Green Energy Bridge", "AEC-2024-002", 0.00m, 1, 850000.00m },
                    { new Guid("8db7a195-25e6-4927-8025-f938b8e0e7a1"), new DateTime(2026, 4, 20, 11, 34, 38, 682, DateTimeKind.Utc).AddTicks(6084), null, 0m, "Skyline Tower Expansion", "AEC-2024-001", 450000.00m, 0, 1250000.00m }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "CreatedBy", "Description", "ProjectId", "TransactionDate", "Type" },
                values: new object[] { new Guid("f4625b87-8d34-450a-9d22-1d54e4f7e27e"), 50000.00m, new DateTime(2026, 4, 20, 11, 34, 38, 684, DateTimeKind.Utc).AddTicks(2586), null, "Concrete Foundation Materials", new Guid("8db7a195-25e6-4927-8025-f938b8e0e7a1"), new DateTime(2026, 4, 10, 11, 34, 38, 684, DateTimeKind.Utc).AddTicks(4954), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("2d5e865a-9310-4828-9717-3f338d38827c"));

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("8db7a195-25e6-4927-8025-f938b8e0e7a1"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f4625b87-8d34-450a-9d22-1d54e4f7e27e"));

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Transactions");

            migrationBuilder.AlterColumn<long>(
                name: "OutputTokens",
                table: "ConversationLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InputTokens",
                table: "ConversationLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
