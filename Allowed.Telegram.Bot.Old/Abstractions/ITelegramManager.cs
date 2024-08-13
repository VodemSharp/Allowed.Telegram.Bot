using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Models;

namespace Allowed.Telegram.Bot.Abstractions;

public interface ITelegramManager
{
    Task Start(ClientItem clients);
    Task Start(IEnumerable<ClientItem> clients);
    Task Stop(string name);
    Task Stop(IEnumerable<string> names);
}