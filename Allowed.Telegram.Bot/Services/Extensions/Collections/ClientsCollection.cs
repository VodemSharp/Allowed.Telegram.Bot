using System.Collections.Generic;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public class ClientsCollection : IClientsCollection
    {
        public List<ClientItem> Clients { get; set; }
    }
}
