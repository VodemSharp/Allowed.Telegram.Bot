using Telegram.Bot.Args;

namespace Allowed.Telegram.Bot.Handlers.MessageHandler
{
    public interface IMessageHandler
    {
        void OnMessage(object sender, MessageEventArgs e);
        void OnCallbackQuery(object sender, CallbackQueryEventArgs e);
    }
}
