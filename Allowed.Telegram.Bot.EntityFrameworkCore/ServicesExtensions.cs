using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Allowed.Telegram.Bot.Data.Entities;
using Allowed.Telegram.Bot.EntityFrameworkCore.Actions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Attributes;
using Allowed.Telegram.Bot.EntityFrameworkCore.Filters;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.EntityFrameworkCore;

public static class ServicesExtensions
{
    public static IServiceCollection AddTelegramEfActions(this IServiceCollection services)
    {
        services.AddTransient(typeof(CheckBotAction));
        services.AddTransient(typeof(CheckUserAction));

        return services;
    }

    public static IServiceCollection AddTelegramEfFilters(this IServiceCollection services)
    {
        return services.AddTransient(typeof(RoleFilter));
    }

    public static IServiceCollection AddTelegramEfAttributes(this IServiceCollection services)
    {
        return services.AddTransient(typeof(StateAttribute));
    }

    public static IServiceCollection AddTelegramEfServices<TContext, TKey, TBot, TUser, TBotUser, TRole, TBotUserRole>
        (this IServiceCollection services)
        where TKey : IEquatable<TKey>
        where TContext : DbContext
        where TBot : TelegramBot, new()
        where TUser : TelegramUser, new()
        where TBotUser : TelegramBotUser, new()
        where TRole : TelegramRole<TKey>, new()
        where TBotUserRole : TelegramBotUserRole<TKey>, new()
    {
        services.AddTransient<IBotService, BotService<TContext, TBot>>();
        services.AddTransient<IRoleService, RoleService<TContext, TKey, TRole, TBotUserRole>>();
        services.AddTransient<IUserService, UserService<TContext, TUser, TBotUser>>();

        return services;
    }

    public static ICommandBuilder AddRoleFilter(this ICommandBuilder builder, string roleName)
    {
        builder.AddFilter<RoleFilter>(roleName);
        return builder;
    }

    public static ICommandGroupBuilder AddRoleFilter(this ICommandGroupBuilder builder, string roleName)
    {
        builder.AddFilter<RoleFilter>(roleName);
        return builder;
    }

    public static ICommandBuilder AddStateAttribute(this ICommandBuilder builder, string state)
    {
        builder.AddAttribute<StateAttribute>(state);
        return builder;
    }
}