using Allowed.Telegram.Bot.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.TelegramServices
{
    public class TelegramService : ITelegramService
    {
        private AllowedTelegramBotDbContext _db { get; set; }

        public TelegramService(AllowedTelegramBotDbContext db)
        {
            _db = db;
        }

        public void CheckUser(long chatId, string username)
        {
            if (!_db.TelegramUsers.Any(u => u.ChatId == chatId))
            {
                _db.TelegramUsers.Add(new TelegramUser { ChatId = chatId, Username = username });
                _db.SaveChanges();
            }
        }

        public TelegramUser GetUser(long chatId)
        {
            return _db.TelegramUsers.FirstOrDefault(u => u.ChatId == chatId);
        }

        public TelegramBot GetBot(string botName)
        {
            TelegramBot bot = _db.TelegramBots.FirstOrDefault(u => u.Name == botName);

            if (!string.IsNullOrEmpty(botName) && bot == null)
            {
                bot = _db.TelegramBots.Add(new TelegramBot { Name = botName }).Entity;
                _db.SaveChanges();
            }

            return bot;
        }

        public TelegramState GetState(long chatId, string botName = null)
        {
            TelegramUser user = GetUser(chatId);

            if (user == null)
                return null;

            if (string.IsNullOrEmpty(botName))
                return _db.TelegramStates.FirstOrDefault(m => m.TelegramUserId == user.Id);
            else
                return _db.TelegramStates.Include(lm => lm.TelegramBot)
                    .FirstOrDefault(m => m.TelegramUserId == user.Id && m.TelegramBot.Name == botName);
        }

        private TelegramState GetStateByUserId(long userId, string botName = null)
        {
            if (string.IsNullOrEmpty(botName))
                return _db.TelegramStates.FirstOrDefault(m => m.TelegramUserId == userId && m.TelegramBotId == null);
            else
                return _db.TelegramStates.Include(lm => lm.TelegramBot)
                    .FirstOrDefault(m => m.TelegramUserId == userId && m.TelegramBot.Name == botName);
        }

        public void SetState(long chatId, string value, string botName = null)
        {
            TelegramUser telegramUser = GetUser(chatId);
            TelegramBot telegramBot = GetBot(botName);
            TelegramState message = GetStateByUserId(telegramUser.Id, botName);

            if (message == null)
            {
                _db.TelegramStates.Add(new TelegramState
                {
                    TelegramUserId = telegramUser.Id,
                    TelegramBotId = telegramBot?.Id,
                    Value = value
                });
            }
            else
            {
                message.Value = value;
                _db.TelegramStates.Update(message);
            }

            _db.SaveChanges();
        }
    }
}
