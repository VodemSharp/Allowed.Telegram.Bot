using Allowed.Telegram.Bot;
using Allowed.Telegram.Bot.Clients;
using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Contexts;
using Allowed.Telegram.Bot.EntityFrameworkCore;
using Allowed.Telegram.Bot.EntityFrameworkCore.Actions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Attributes;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Sample.Contexts;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection).UseSnakeCaseNamingConvention());

builder.Services.AddTelegramServices();
builder.Services.AddTelegramHandler();

builder.Services.AddTelegramEfServices<ApplicationDbContext, int, ApplicationTgBot, ApplicationTgUser,
    ApplicationTgBotUser, ApplicationTgRole, ApplicationTgBotUserRole>();
builder.Services.AddTelegramEfActions();
builder.Services.AddTelegramEfFilters();
builder.Services.AddTelegramEfAttributes();

var app = builder.Build();

app.AddAttributeHandler<StateAttribute>();
app.AddGlobalActionBefore<CheckBotAction>();
app.AddGlobalActionBefore<CheckUserAction>();

// More examples in no db sample
app.MapMessageCommand("/start",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You pressed: /start");
    });

app.MapMessageCommand("/start",
    async (ITelegramBotClient client, Message message, MessageCommandArgs args) =>
    {
        var result = "You pressed: /start";

        // This check makes sense only without "Strict" implementation
        if (!string.IsNullOrEmpty(args.Value))
            result = $"{result}\nArgs: {args.Value}";

        await client.SendTextMessageAsync(message.From!.Id, result);
    }, MessageCommandCheckTypes.Parameterized);

app.MapMessageCommand("/add_admin_role",
    async (ITelegramBotClient client, Message message, IRoleService roleService) =>
    {
        if (!await roleService.Any("admin")) await roleService.Add("admin");

        if (await roleService.Any(client.BotId!.Value, message.From!.Id, "admin"))
        {
            await client.SendTextMessageAsync(message.From!.Id, "You already have admin role!");
        }
        else
        {
            await roleService.Add(client.BotId.Value, message.From!.Id, "admin");
            await client.SendTextMessageAsync(message.From!.Id, "You added admin role!");
        }
    });

app.MapMessageCommand("/remove_admin_role",
    async (ITelegramBotClient client, Message message, IRoleService roleService) =>
    {
        if (await roleService.Any(client.BotId!.Value, message.From!.Id, "admin"))
            await roleService.Remove(client.BotId.Value, message.From!.Id, "admin");

        await client.SendTextMessageAsync(message.From!.Id, "You removed admin role!");
    });

app.MapMessageCommand("/admin_role_message", async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.From!.Id,
        "You have permission for this command in controller!");
}).AddRoleFilter("admin");

/* TextController */
app.MapMessageCommand(async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.From!.Id, $"You say: {message.Text}");
});

app.MapMessageCommand("Test text command",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command!");
    });

app.MapMessageCommand("Test text command 2",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command 2!");
    });

app.MapMessageCommand("/set_text_test_state",
    async (ITelegramBotClient client, Message message, IUserService userService) =>
    {
        await userService.SetState(client.BotId!.Value, message.From!.Id, "TextTestState");
        await client.SendTextMessageAsync(message.From!.Id, "Text test state was set!");
    });

app.MapMessageCommand(async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.From!.Id, "You called text test state method!");
}).AddStateAttribute("TextTestState");

app.MapMessageCommand("Test state command",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You called text test state method with selected text!");
    }).AddStateAttribute("TextTestState");

app.MapMessageCommand("Test parameterized command",
        async (ITelegramBotClient client, Message message) =>
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "You call text test state parameterized method with selected text!");
        }, MessageCommandCheckTypes.Parameterized)
    .AddStateAttribute("TextTestState");

var telegramHandler = app.Services.GetRequiredService<TelegramHandler>();

var bots = new Dictionary<string, string>
{
    { "<TOKEN>", "<PUBLIC_URL>" }
};

foreach (var bot in bots)
{
    var botHandler = TelegramContextFactory.CreateHandler(new SafeTelegramBotClientOptions(bot.Key));
    await botHandler.Client.DeleteWebhookAsync();

    if (app.Environment.IsDevelopment())
    {
        botHandler.Client.StartReceiving(telegramHandler.HandlePollingUpdate, telegramHandler.PollingErrorHandler);
    }
    else
    {
        telegramHandler.Register(botHandler);
        await botHandler.Client.SetWebhookAsync($"{bot.Value}/{bot.Key.Split(':')[0]}");
    }
}

if (!app.Environment.IsDevelopment())
    app.MapPost("/{botId:long}", async ([FromRoute] long botId, HttpContext context, ILogger<Program> logger) =>
    {
        try
        {
            using var streamReader = new StreamReader(context.Request.Body);
            var stream = await streamReader.ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(stream);
            await telegramHandler.HandleWebHookUpdate(botId, update!);
        }
        catch (Exception ex)
        {
            logger.LogError("{ex}", ex.ToString());
        }
    });

app.Run();