using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AECPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("2d5e865a-9310-4828-9717-3f338d38827c"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 20, 11, 37, 54, 480, DateTimeKind.Utc).AddTicks(1283));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("8db7a195-25e6-4927-8025-f938b8e0e7a1"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 20, 11, 37, 54, 479, DateTimeKind.Utc).AddTicks(7202));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f4625b87-8d34-450a-9d22-1d54e4f7e27e"),
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2026, 4, 20, 11, 37, 54, 481, DateTimeKind.Utc).AddTicks(1954), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("2d5e865a-9310-4828-9717-3f338d38827c"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 20, 11, 34, 38, 683, DateTimeKind.Utc).AddTicks(309));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("8db7a195-25e6-4927-8025-f938b8e0e7a1"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 20, 11, 34, 38, 682, DateTimeKind.Utc).AddTicks(6084));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f4625b87-8d34-450a-9d22-1d54e4f7e27e"),
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2026, 4, 20, 11, 34, 38, 684, DateTimeKind.Utc).AddTicks(2586), new DateTime(2026, 4, 10, 11, 34, 38, 684, DateTimeKind.Utc).AddTicks(4954) });
        }
    }
}
