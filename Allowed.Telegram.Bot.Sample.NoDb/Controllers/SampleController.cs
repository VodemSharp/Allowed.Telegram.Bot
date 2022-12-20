using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Sample.NoDb.Controllers;

[BotName("Sample")]
public class SampleController : CommandController
{
    [Command("start")]
    public async Task Start(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You pressed: /start");
    }

    [DefaultCommand]
    public async Task DefaultCommand(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id,
            $"You pressed unknown command: {data.Message.Text}");
    }

    [TextCommand]
    public async Task TextCommand(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id, $"You say: {data.Message.Text}");
    }

    [TextCommand("Test text command")]
    public async Task TestTextMessage(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You have selected a test text command!");
    }

    [TextCommand("Test text command 2")]
    public async Task TestTextMessage2(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You have selected a test text command 2!");
    }
}