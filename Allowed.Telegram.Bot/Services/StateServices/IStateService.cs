namespace Allowed.Telegram.Bot.Services.StateServices
{
    public interface IStateService<TState>
        where TState : class
    {
        TState GetState(long chatId, string botName = "");
        void SetState(long chatId, string value, string botName = "");
    }
}
