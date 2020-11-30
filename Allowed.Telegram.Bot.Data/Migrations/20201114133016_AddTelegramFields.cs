using Microsoft.EntityFrameworkCore.Migrations;

namespace Allowed.Telegram.Bot.Data.Migrations
{
    public partial class AddTelegramFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelegramUsers_ChatId",
                table: "TelegramUsers");

            migrationBuilder.DropIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "TelegramUsers",
                newName: "TelegramId");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TelegramUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "TelegramUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "TelegramUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_TelegramId",
                table: "TelegramUsers",
                column: "TelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelegramUsers_TelegramId",
                table: "TelegramUsers");

            migrationBuilder.DropIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "TelegramUsers");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "TelegramUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_ChatId",
                table: "TelegramUsers",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers",
                column: "Username",
                unique: true);
        }
    }
}
