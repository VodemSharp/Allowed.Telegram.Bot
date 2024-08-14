namespace Allowed.Telegram.Bot.Sample.Controllers;

// [Route("telegram")]
// public class TelegramController : TelegramDbControllerBase<int, ApplicationTgUser, ApplicationTgRole, ApplicationTgBot>
// {
//     private readonly ITelegramManager _telegramManager;
//     
//     private readonly ApplicationDbContext _db;
//     private readonly ClientsCollection _clientsCollection;
//
//     public TelegramController(IServiceProvider serviceProvider, IServiceFactory serviceFactory,
//         ITelegramManager telegramManager,
//         ApplicationDbContext db, ClientsCollection clientsCollection,
//         ControllersCollection controllersCollection, ContextOptions contextOptions) : base(serviceProvider, serviceFactory,
//         clientsCollection, controllersCollection, contextOptions)
//     {
//         _telegramManager = telegramManager;
//         
//         _db = db;
//         _clientsCollection = clientsCollection;
//     }
//
//     // Bots settings (need protect by auth or ip filter)
//     [HttpGet("status")]
//     public async Task<List<TelegramBotDto>> GetStatuses()
//     {
//         var telegramBots = await _db.TelegramBots.ToListAsync();
//         var activeBots = _clientsCollection.Clients.Select(c => c.Options.Name).ToList();
//
//         return telegramBots.Select(b => new TelegramBotDto
//         {
//             Name = b.Name,
//             Started = activeBots.Contains(b.Name)
//         }).ToList();
//     }
//
//     [HttpPost("start/{name}/{token}")]
//     public async Task Start(string name, string token)
//     {
//         await _telegramManager.Start(TelegramBotClientFactory.CreateClient(new SafeTelegramBotClientOptions(name, token)));
//     }
//
//     [HttpPost("stop/{name}")]
//     public async Task Start(string name)
//     {
//         await _telegramManager.Stop(name);
//     }
// }