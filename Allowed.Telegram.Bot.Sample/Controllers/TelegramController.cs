using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.EntityFrameworkCore.Controllers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.AspNetCore.Mvc;

namespace Allowed.Telegram.Bot.Sample.Controllers;

[Route("telegram")]
public class TelegramController : TelegramDbControllerBase<int, ApplicationTgUser, ApplicationTgRole>
{
    public TelegramController(IServiceProvider serviceProvider, IServiceFactory serviceFactory,
        ControllersCollection controllersCollection, ClientsCollection clientsCollection,
        BotsCollection<int> botsCollection) : base(serviceProvider, serviceFactory, controllersCollection,
        clientsCollection, botsCollection)
    {
    }
}