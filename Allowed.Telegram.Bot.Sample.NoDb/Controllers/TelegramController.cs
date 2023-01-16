using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Factories;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Sample.NoDb.Models;
using Microsoft.AspNetCore.Mvc;

namespace Allowed.Telegram.Bot.Sample.NoDb.Controllers;

[Route("telegram")]
public class TelegramController : TelegramControllerBase
{
    private readonly ITelegramManager _telegramManager;
    private readonly ClientsCollection _clientsCollection;

    public TelegramController(IServiceProvider serviceProvider, ITelegramManager telegramManager,
        ControllersCollection controllersCollection,
        ClientsCollection clientsCollection) : base(serviceProvider, controllersCollection, clientsCollection)
    {
        _telegramManager = telegramManager;
        _clientsCollection = clientsCollection;
    }

    // Bots settings (need protect by auth or ip filter)
    [HttpGet("status")]
    public List<TelegramBotDto> GetStatuses()
    {
        var activeBots = _clientsCollection.Clients.Select(c => c.Options.Name).ToList();

        return activeBots.Select(b => new TelegramBotDto
        {
            Name = b,
            Started = true
        }).ToList();
    }

    [HttpPost("start/{name}/{token}")]
    public async Task Start(string name, string token)
    {
        await _telegramManager.Start(
            TelegramBotClientFactory.CreateClient(new SafeTelegramBotClientOptions(name, token)));
    }

    [HttpPost("stop/{name}")]
    public async Task Start(string name)
    {
        await _telegramManager.Stop(name);
    }
}