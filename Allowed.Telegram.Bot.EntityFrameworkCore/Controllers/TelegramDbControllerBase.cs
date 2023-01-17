using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Controllers;

public abstract class TelegramDbControllerBase<TKey, TUser, TRole, TBot> : ControllerBase
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
    where TBot: TelegramBot<TKey>
{
    private readonly ClientsCollection _clientsCollection;
    private readonly ControllersCollection _controllersCollection;
    private readonly ContextOptions _contextOptions;
    private readonly IServiceFactory _serviceFactory;
    private readonly IServiceProvider _serviceProvider;

    public TelegramDbControllerBase(IServiceProvider serviceProvider, IServiceFactory serviceFactory,
        ClientsCollection clientsCollection, ControllersCollection controllersCollection, ContextOptions contextOptions)
    {
        _serviceProvider = serviceProvider;
        _serviceFactory = serviceFactory;
        _controllersCollection = controllersCollection;
        _clientsCollection = clientsCollection;
        _contextOptions = contextOptions;
    }

    [HttpPost("{token}")]
    public async Task Post([FromBody] Update update, string token)
    {
        var client = _clientsCollection.Clients.Single(c => c.Options.Token == token);
        var db = (DbContext)_serviceProvider.GetRequiredService(_contextOptions.ContextType);

        var botId = await db.Set<TBot>().Where(b => b.Name == client.Options.Name)
            .Select(b => b.Id).SingleAsync();

        var userService = _serviceFactory.CreateUserService<TKey, TUser>(botId);
        var roleService = _serviceFactory.CreateRoleService<TKey, TRole>(botId);

        var messageHandler = new MessageDbHandler<TKey, TUser, TRole>(_controllersCollection,
            client.Client, client.Options, userService, roleService, _serviceProvider);

        await messageHandler.OnUpdate(client.Client, update, botId, new CancellationToken());
    }
}