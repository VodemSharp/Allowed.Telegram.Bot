using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public interface IClientsCollection
    {
        List<ClientItem> Clients { get; set; }
    }
}
