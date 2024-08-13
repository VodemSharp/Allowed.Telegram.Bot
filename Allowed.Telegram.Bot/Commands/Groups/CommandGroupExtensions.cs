using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Groups;

public static class CommandGroupExtensions
{
    public static ICommandGroupBuilder MapCommandGroup(this IHost host)
    {
        var group = new CommandGroup
        {
            Host = host
        };
        
        return new CommandGroupBuilder(group);
    }
    
    public static ICommandGroupBuilder MapCommandGroup(this ICommandGroupBuilder builder)
    {
        return new CommandGroupBuilder(builder);
    }
}