using Allowed.Telegram.Bot;
using Allowed.Telegram.Bot.Clients.Options;
using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.EntityFrameworkCore;
using Allowed.Telegram.Bot.EntityFrameworkCore.Actions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Attributes;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Sample.Contexts;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection).UseSnakeCaseNamingConvention());

builder.Services.AddTelegramServices();

builder.Services.AddTelegramEfServices<ApplicationDbContext, int, ApplicationTgBot, ApplicationTgUser,
    ApplicationTgBotUser, ApplicationTgRole, ApplicationTgBotUserRole>();
builder.Services.AddTelegramEfActions();
builder.Services.AddTelegramEfFilters();
builder.Services.AddTelegramEfAttributes();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTelegramManager();
// TODO
// else
//     builder.Services.AddTelegramDbWebHookManager();

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
    }, MessageCommandTypes.Parameterized);

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
    }, MessageCommandTypes.Parameterized)
    .AddStateAttribute("TextTestState");

var telegramManager = app.Services.GetRequiredService<ITelegramManager>();
await telegramManager.Start(new[]
{
    TelegramHandlerFactory.CreateHandler(new SimpleTelegramBotClientOptions("<TOKEN>"))
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();