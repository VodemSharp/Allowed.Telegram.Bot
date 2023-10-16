using Microsoft.EntityFrameworkCore.Migrations;

namespace Allowed.Telegram.Bot.Sample.Migrations
{
    public partial class AddBotBlocked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BotBlocked",
                table: "TelegramBotUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotBlocked",
                table: "TelegramBotUsers");
        }
    }
}
