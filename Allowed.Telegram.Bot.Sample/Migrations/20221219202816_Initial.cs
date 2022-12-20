using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Allowed.Telegram.Bot.Sample.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telegram_bots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "telegram_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "telegram_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegramid = table.Column<long>(name: "telegram_id", type: "bigint", nullable: false),
                    username = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(name: "first_name", type: "text", nullable: true),
                    lastname = table.Column<string>(name: "last_name", type: "text", nullable: true),
                    languagecode = table.Column<string>(name: "language_code", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "telegram_bot_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegramuserid = table.Column<int>(name: "telegram_user_id", type: "integer", nullable: false),
                    telegrambotid = table.Column<int>(name: "telegram_bot_id", type: "integer", nullable: false),
                    botblocked = table.Column<bool>(name: "bot_blocked", type: "boolean", nullable: false),
                    state = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bot_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_telegram_bot_users_telegram_bots_application_tg_bot_id",
                        column: x => x.telegrambotid,
                        principalTable: "telegram_bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telegram_bot_users_telegram_users_application_tg_user_id",
                        column: x => x.telegramuserid,
                        principalTable: "telegram_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegramuserid = table.Column<int>(name: "telegram_user_id", type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_files_telegram_users_telegram_user_id",
                        column: x => x.telegramuserid,
                        principalTable: "telegram_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_bot_user_roles",
                columns: table => new
                {
                    telegrambotuserid = table.Column<int>(name: "telegram_bot_user_id", type: "integer", nullable: false),
                    telegramroleid = table.Column<int>(name: "telegram_role_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bot_user_roles", x => new { x.telegrambotuserid, x.telegramroleid });
                    table.ForeignKey(
                        name: "fk_telegram_bot_user_roles_telegram_bot_users_application_tg_b",
                        column: x => x.telegrambotuserid,
                        principalTable: "telegram_bot_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telegram_bot_user_roles_telegram_roles_application_tg_role_",
                        column: x => x.telegramroleid,
                        principalTable: "telegram_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_telegram_bot_user_roles_telegram_role_id",
                table: "telegram_bot_user_roles",
                column: "telegram_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_bot_users_telegram_bot_id",
                table: "telegram_bot_users",
                column: "telegram_bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_bot_users_telegram_user_id",
                table: "telegram_bot_users",
                column: "telegram_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_users_telegram_id",
                table: "telegram_users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_users_username",
                table: "telegram_users",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "ix_user_files_telegram_user_id",
                table: "user_files",
                column: "telegram_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_bot_user_roles");

            migrationBuilder.DropTable(
                name: "user_files");

            migrationBuilder.DropTable(
                name: "telegram_bot_users");

            migrationBuilder.DropTable(
                name: "telegram_roles");

            migrationBuilder.DropTable(
                name: "telegram_bots");

            migrationBuilder.DropTable(
                name: "telegram_users");
        }
    }
}
