using System.Text.Json.Serialization;
using Allowed.Telegram.Bot;
using Allowed.Telegram.Bot.Clients;
using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;
using Allowed.Telegram.Bot.Commands.Execution.ChannelPosts;
using Allowed.Telegram.Bot.Commands.Execution.ChosenInlineResults;
using Allowed.Telegram.Bot.Commands.Execution.EditedChannelPosts;
using Allowed.Telegram.Bot.Commands.Execution.EditedMessages;
using Allowed.Telegram.Bot.Commands.Execution.InlineQueries;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Execution.Updates;
using Allowed.Telegram.Bot.Commands.Groups;
using Allowed.Telegram.Bot.Contexts;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Sample.NoDb.Actions;
using Allowed.Telegram.Bot.Sample.NoDb.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelegramServices();
builder.Services.AddTelegramHandler();

builder.Services.AddTransient<TestFilter>();
builder.Services.AddTransient<TestAction>();
builder.Services.AddTransient<CheckAction>();

var app = builder.Build();

app.AddGlobalActionBefore<CheckAction>();

#region Message commands

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
    }, MessageCommandCheckTypes.Parameterized);

app.MapMessageCommand(
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendPhotoAsync(message.From!.Id,
            InputFile.FromFileId(message.Photo!.MaxBy(x => x.FileSize)!.FileId));
    }, MessageType.Photo);

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
    }, MessageCommandCheckTypes.Parameterized);

// If you type for example "Test text command 2" you will get the previous command
// because this text is also correct for previous method
// Parameterized text commands are recommended only for different initial texts
group.MapMessageCommand("Test text command 2",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command 2!");
    }, MessageCommandCheckTypes.Parameterized);

group.MapMessageCommand("Test text command 3",
    async (ITelegramBotClient client, Message message) =>
    {
        await client.SendTextMessageAsync(message.From!.Id, "You have selected a test text command 3!");
    });

#endregion

#region Inline commands

var companies = new[] { "Apple", "Microsoft", "Google", "Samsung" };
app.MapInlineQueryCommand(async (ITelegramBotClient client, InlineQuery inlineQuery) =>
{
    await client.AnswerInlineQueryAsync(inlineQuery.Id, companies.Where(x => x.Contains(inlineQuery.Query))
        .Select(x => new InlineQueryResultArticle(x, x, new InputTextMessageContent(x))));
});

#endregion

#region Chosen inline result commands

app.MapChosenInlineResultCommand(async (ITelegramBotClient client, ChosenInlineResult chosenInlineResult) =>
{
    await client.SendTextMessageAsync(chosenInlineResult.From.Id, $"Your choice: {chosenInlineResult.ResultId}");
});

#endregion

#region Callback query commands

app.MapMessageCommand("/query", async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(
        message.From!.Id,
        "Callback query",
        replyMarkup: new InlineKeyboardMarkup(
            new List<List<InlineKeyboardButton>>
            {
                new()
                {
                    new InlineKeyboardButton("True")
                    {
                        CallbackData =
                            new TestCallbackQueryModel
                            {
                                Path = "test",
                                SomeData = true
                            }
                    },
                    new InlineKeyboardButton("False")
                    {
                        CallbackData =
                            new TestCallbackQueryModel
                            {
                                Path = "test",
                                SomeData = false
                            }
                    },
                    new InlineKeyboardButton("Default")
                    {
                        CallbackData =
                            new CallbackQueryModel
                            {
                                Path = "default"
                            }
                    }
                }
            }
        )
    );
});

app.MapCallbackQueryCommand("test",
    async (ITelegramBotClient client, CallbackQuery callbackQuery, TestCallbackQueryModel model) =>
    {
        await client.SendTextMessageAsync(callbackQuery.Message!.Chat.Id, $"Model: {model.SomeData}");
        await client.AnswerCallbackQueryAsync(callbackQuery.Id);
    });

app.MapCallbackQueryCommand(async (ITelegramBotClient client, CallbackQuery callbackQuery) =>
{
    await client.SendTextMessageAsync(callbackQuery.Message!.Chat.Id, "Callback Default Query");
    await client.AnswerCallbackQueryAsync(callbackQuery.Id);
});

#endregion

#region Edited message commands

app.MapEditedMessageCommand(async (ITelegramBotClient client, Message editedMessage) =>
{
    await client.SendTextMessageAsync(editedMessage.From!.Id, $"Your edited message: {editedMessage.Text}");
});

#endregion

#region Channel post commands

app.MapChannelPostCommand(async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.Chat.Id, $"Your message: {message.Text}");
});

#endregion

#region Edited post commands

app.MapEditedChannelPostCommand(async (ITelegramBotClient client, Message message) =>
{
    await client.SendTextMessageAsync(message.Chat.Id, $"Your edited message: {message.Text}");
});

#endregion

#region Update commands

app.MapUpdateCommand((ITelegramBotClient _, Update _) =>
{
    // Additional way of processing commands
});

#endregion

var telegramHandler = app.Services.GetRequiredService<TelegramHandler>();
var bots = new Dictionary<string, string>
{
    { "<TOKEN>", "<PUBLIC_URL>" }
};

foreach (var bot in bots)
{
    var allowedBot = TelegramContextFactory.CreateHandler(new SafeTelegramBotClientOptions(bot.Key));
    await allowedBot.Client.DeleteWebhookAsync();

    if (app.Environment.IsDevelopment())
    {
        allowedBot.Client.StartReceiving(telegramHandler.HandlePollingUpdate, telegramHandler.PollingErrorHandler);
    }
    else
    {
        telegramHandler.Register(allowedBot);
        await allowedBot.Client.SetWebhookAsync($"{bot.Value}/{bot.Key.Split(':')[0]}");
        
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
    }
}

app.Run();

internal class TestCallbackQueryModel : CallbackQueryModel
{
    [JsonPropertyName("a")] public bool SomeData { get; set; }
}