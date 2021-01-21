using Allowed.Telegram.Bot.Data.Models;
using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Data.Services
{
    public interface IStateService<TKey, TState>
        where TKey : IEquatable<TKey>
        where TState : TelegramState<TKey>
    {
        Task<TState> GetState(string username);
        Task<TState> GetState(long telegramId);
        Task SetState(string username, string value);
        Task SetState(long telegramId, string value);
    }
}
