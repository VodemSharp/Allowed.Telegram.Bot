using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Controllers;

public abstract class TelegramControllerBase : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ControllersCollection _controllersCollection;
    private readonly ClientsCollection _clientsCollection;
    
    public TelegramControllerBase(IServiceProvider serviceProvider,
        ControllersCollection controllersCollection,
        ClientsCollection clientsCollection)
    {
        _serviceProvider = serviceProvider;
        _controllersCollection = controllersCollection;
        _clientsCollection = clientsCollection;
    }

    private MessageHandler GetMessageHandler(ITelegramBotClient client, SimpleTelegramBotClientOptions options)
    {
        return new MessageHandler(_controllersCollection, client, options, _serviceProvider);
    }

    [HttpPost("{token}")]
    public virtual async Task Post([FromBody] Update update, string token)
    {
        var client = _clientsCollection.Clients.Single(c => c.Options.Token == token);
        await GetMessageHandler(client.Client, client.Options).OnUpdate(client.Client, update, new CancellationToken());
    }
}