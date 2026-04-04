using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TgRobuxBot.Migrations
{
    /// <inheritdoc />
    public partial class AddRobux540to780 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Name", "PriceUZS" },
                values: new object[,]
                {
                    { 22, "Robux", "540 Robux", 76000m },
                    { 23, "Robux", "580 Robux", 83000m },
                    { 24, "Robux", "620 Robux", 90000m },
                    { 25, "Robux", "660 Robux", 97000m },
                    { 26, "Robux", "700 Robux", 104000m },
                    { 27, "Robux", "740 Robux", 111000m },
                    { 28, "Robux", "780 Robux", 118000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);
        }
    }
}
