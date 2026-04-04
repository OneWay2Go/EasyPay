using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TgRobuxBot.Migrations
{
    /// <inheritdoc />
    public partial class ComplexProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempData",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "Robux", "120 Robux", 21000m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "Robux", "160 Robux", 28000m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "Robux", "200 Robux", 35000m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Name", "PriceUZS" },
                values: new object[,]
                {
                    { 6, "Robux", "240 Robux", 42000m },
                    { 7, "Robux", "280 Robux", 49000m },
                    { 8, "Robux", "320 Robux", 56000m },
                    { 9, "Robux", "360 Robux", 63000m },
                    { 10, "Robux", "400 Robux", 70000m },
                    { 11, "Robux", "500 Robux", 69000m },
                    { 12, "Robux", "1000 Robux", 135000m },
                    { 13, "Robux", "1700+300 Robux", 260000m },
                    { 14, "Robux", "4500+750 Robux", 630000m },
                    { 15, "Robux", "10.000+1000 Robux", 1250000m },
                    { 16, "Robux", "22.500+1500 Robux", 2500000m },
                    { 17, "TelegramPremium", "1 oy (Login orqali)", 42000m },
                    { 18, "TelegramPremium", "3 oy (Gift)", 166000m },
                    { 19, "TelegramPremium", "6 oy (Gift)", 220000m },
                    { 20, "TelegramPremium", "12 oy (Gift)", 390000m },
                    { 21, "TelegramStars", "Telegram Stars (Maxsus)", 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DropColumn(
                name: "TempData",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "TelegramStars", "50 Stars", 12000m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "TelegramStars", "100 Stars", 24000m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Name", "PriceUZS" },
                values: new object[] { "TelegramPremium", "1 Month Premium", 42000m });
        }
    }
}
