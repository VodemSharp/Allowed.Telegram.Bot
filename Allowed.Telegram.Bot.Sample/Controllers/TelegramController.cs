using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.EntityFrameworkCore.Controllers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Factories;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Sample.Contexts;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Allowed.Telegram.Bot.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Controllers;

[Route("telegram")]
public class TelegramController : TelegramDbControllerBase<int, ApplicationTgUser, ApplicationTgRole>
{
    private readonly ITelegramManager _telegramManager;
    
    private readonly ApplicationDbContext _db;
    private readonly ClientsCollection _clientsCollection;

    public TelegramController(IServiceProvider serviceProvider, IServiceFactory serviceFactory,
        ITelegramManager telegramManager,
        ApplicationDbContext db, ClientsCollection clientsCollection,
        ControllersCollection controllersCollection,
        BotsCollection<int> botsCollection) : base(serviceProvider, serviceFactory,
        clientsCollection, controllersCollection, botsCollection)
    {
        _telegramManager = telegramManager;
        
        _db = db;
        _clientsCollection = clientsCollection;
    }

    // Bots settings (need protect by auth or ip filter)
    [HttpGet("status")]
    public async Task<List<TelegramBotDto>> GetStatuses()
    {
        var telegramBots = await _db.TelegramBots.ToListAsync();
        var activeBots = _clientsCollection.Clients.Select(c => c.Options.Name).ToList();

        return telegramBots.Select(b => new TelegramBotDto
        {
            Name = b.Name,
            Started = activeBots.Contains(b.Name)
        }).ToList();
    }

    [HttpPost("start/{name}/{token}")]
    public async Task Start(string name, string token)
    {
        await _telegramManager.Start(TelegramBotClientFactory.CreateClient(new SafeTelegramBotClientOptions(name, token)));
    }

    [HttpPost("stop/{name}")]
    public async Task Start(string name)
    {
        await _telegramManager.Stop(name);
    }
}