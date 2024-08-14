using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Attributes;

public static class CommandAttributeExtensions
{
    public static IHost AddAttributeHandler<THandler>(this IHost app)
        where THandler : CommandAttributeHandler
    {
        var collection = app.Services.GetRequiredService<CommandAttributeHandlerCollection>();
        collection.Handlers.Add(typeof(THandler));
        return app;
    }
}