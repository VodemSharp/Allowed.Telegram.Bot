using Allowed.Telegram.Bot;
using Allowed.Telegram.Bot.Clients.Options;
using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Execution.Updates;
using Allowed.Telegram.Bot.Commands.Groups;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Sample.NoDb.Actions;
using Allowed.Telegram.Bot.Sample.NoDb.Filters;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTelegramServices();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTelegramManager();

builder.Services.AddTransient<TestFilter>();
builder.Services.AddTransient<TestAction>();
builder.Services.AddTransient<CheckAction>();

var app = builder.Build();

app.AddGlobalActionBefore<CheckAction>();

app.MapMessageCommand(async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.From!.Id, "Default message!");
});

// You can use only parameterized example for strict as well
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

var group = app.MapCommandGroup()
    .AddFilter<TestFilter>(true)
    .AddActionAfter<TestAction>();

group.MapMessageCommand("/filter",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You pressed: /filter");
    });

group.MapMessageCommand("Test text command",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command!");
    }, MessageCommandTypes.Parameterized);

// If you type for example "Test text command 2" you will get the previous command
// because this text is also correct for previous method
// Parameterized text commands are recommended only for different initial texts
group.MapMessageCommand("Test text command 2",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command 2!");
    }, MessageCommandTypes.Parameterized);

group.MapMessageCommand("Test text command 3",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command 3!");
    });

app.MapUpdateCommand(async (ITelegramBotClient client, Update update) =>
{
    if (update.Type == UpdateType.EditedMessage)
        await client.SendTextMessageAsync(update.EditedMessage!.Chat,
            "I don't know how to handle this command in a standard way yet!");
});

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