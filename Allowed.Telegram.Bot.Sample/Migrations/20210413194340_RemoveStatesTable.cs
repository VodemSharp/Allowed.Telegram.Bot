using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Allowed.Telegram.Bot.Sample.Migrations
{
    public partial class RemoveStatesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramStates");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TelegramBotUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "TelegramBotUsers");

            migrationBuilder.CreateTable(
                name: "TelegramStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramBotUserId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramStates_TelegramBotUsers_TelegramBotUserId",
                        column: x => x.TelegramBotUserId,
                        principalTable: "TelegramBotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramStates_TelegramBotUserId",
                table: "TelegramStates",
                column: "TelegramBotUserId");
        }
    }
}
