using Allowed.Telegram.Bot.Models.Store;
using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services.StateServices
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
