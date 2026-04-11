using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifyBotAndBackend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotState",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BotTempData",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptFileId",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotState",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BotTempData",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceiptFileId",
                table: "Orders");
        }
    }
}
