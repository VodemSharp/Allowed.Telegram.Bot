using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Controllers;

public abstract class TelegramDbControllerBase<TKey, TUser, TRole> : ControllerBase
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
{
    private readonly BotsCollection<TKey> _botsCollection;
    private readonly ControllersCollection _controllersCollection;
    private readonly ClientsCollection _clientsCollection;
    private readonly IServiceFactory _serviceFactory;
    private readonly IServiceProvider _serviceProvider;

    public TelegramDbControllerBase(IServiceProvider serviceProvider, IServiceFactory serviceFactory,
        ClientsCollection clientsCollection, ControllersCollection controllersCollection,
        BotsCollection<TKey> botsCollection)
    {
        _serviceProvider = serviceProvider;
        _serviceFactory = serviceFactory;
        _controllersCollection = controllersCollection;
        _clientsCollection = clientsCollection;
        _botsCollection = botsCollection;
    }

    [HttpPost("{token}")]
    public async Task Post([FromBody] Update update, string token)
    {
        var client = _clientsCollection.Clients.Single(c => c.Options.Token == token);

        var botId = _botsCollection.Values.GetValueOrDefault(client.Options.Name);

        var userService = _serviceFactory.CreateUserService<TKey, TUser>(botId);
        var roleService = _serviceFactory.CreateRoleService<TKey, TRole>(botId);

        var messageHandler = new MessageDbHandler<TKey, TUser, TRole>(_controllersCollection,
            client.Client, client.Options, userService, roleService, _serviceProvider);

        await messageHandler.OnUpdate(client.Client, update, botId, new CancellationToken());
    }
}