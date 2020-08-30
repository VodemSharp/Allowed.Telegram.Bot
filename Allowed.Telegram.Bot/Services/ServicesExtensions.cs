using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services
{
    public static class ServicesExtensions
    {
        public static async Task<TKey> GetBotUserId<TKey>(this DbContext db, TKey botId, long chatId)
        {
            TKey result;

            var connection = db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT t2.Id "
                  + "FROM TelegramUsers AS t1 "
                  + "INNER JOIN TelegramBotUsers AS t2 ON t1.Id = t2.TelegramUserId "
                  + $"WHERE t1.ChatId = {chatId} AND t2.TelegramBotId = {botId} "
                  + "LIMIT 1";

                result = (TKey)await command.ExecuteScalarAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
            return result;
        }

        public static async Task<TKey> GetBotUserId<TKey>(this DbContext db, TKey botId, string username)
        {
            TKey result;

            var connection = db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT t2.Id "
                  + "FROM TelegramUsers AS t1 "
                  + "INNER JOIN TelegramBotUsers AS t2 ON t1.Id = t2.TelegramUserId "
                  + $"WHERE t1.Username = '{username}' AND t2.TelegramBotId = {botId} "
                  + "LIMIT 1";

                result = (TKey)await command.ExecuteScalarAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
            return result;
        }
    }
}
