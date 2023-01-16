using Allowed.Telegram.Bot.Extensions.Collections.Items;

namespace Allowed.Telegram.Bot.Extensions.Collections;

public class ClientsCollection
{
    public List<ClientItem> Clients { get; set; } = new();
}