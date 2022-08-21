using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services;

public class RoleService<TKey, TRole> : IRoleService<TKey, TRole>
    where TKey : IEquatable<TKey>
    where TRole : TelegramRole<TKey>
{
    private readonly TKey _botId;
    private readonly IQueryable<TelegramBotUser<TKey>> _botUsers;
    private readonly DbContext _db;
    private readonly ContextOptions _options;
    private readonly IQueryable<TelegramRole<TKey>> _roles;

    private readonly IQueryable<TelegramBotUserRole<TKey>> _userRoles;
    private readonly IQueryable<TelegramUser<TKey>> _users;

    public RoleService(IServiceProvider provider, ContextOptions options, TKey botId)
    {
        _db = (DbContext)provider.GetService(options.ContextType);
        _options = options;
        _botId = botId;

        _userRoles = _db.Set(_options.BotUserRoleType).Cast<TelegramBotUserRole<TKey>>();
        _roles = _db.Set(_options.RoleType).Cast<TelegramRole<TKey>>();
        _users = _db.Set(_options.UserType).Cast<TelegramUser<TKey>>();
        _botUsers = _db.Set(_options.BotUserType).Cast<TelegramBotUser<TKey>>();
    }

    public async Task<bool> AnyRole(string role)
    {
        return await _db.Set<TRole>().AnyAsync(r => r.Name == role);
    }

    public async Task<TRole> GetRole(string role)
    {
        return await _db.Set<TRole>().SingleAsync(r => r.Name == role);
    }

    public async Task AddRole(TRole role)
    {
        await _db.Set<TRole>().AddAsync(role);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateRole(TRole role)
    {
        _db.Set<TRole>().Update(role);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveRole(TRole role)
    {
        _db.Set<TRole>().Remove(role);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> AnyUserRole(long telegramId, string role)
    {
        return await (from r in _roles
                join ur in _userRoles on r.Id equals ur.TelegramRoleId
                join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
                join u in _users on bu.TelegramUserId equals u.Id
                where u.TelegramId == telegramId && bu.TelegramBotId.Equals(_botId) && r.Name == role
                select r)
            .AnyAsync();
    }

    public async Task<bool> AnyUserRole(string username, string role)
    {
        return await (from r in _roles
                join ur in _userRoles on r.Id equals ur.TelegramRoleId
                join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
                join u in _users on bu.TelegramUserId equals u.Id
                where u.Username == username && bu.TelegramBotId.Equals(_botId) && r.Name == role
                select r)
            .AnyAsync();
    }

    public async Task AddUserRole(long telegramId, string role)
    {
        var botUserId = await LinqHelper.GetBotUserId(_users, _botUsers, _botId, telegramId);
        var roleId = (await _db.Set<TRole>().OrderBy(r => r.Id).SingleAsync(r => r.Name == role)).Id;

        await _db.AddAsync(ContextBuilder.CreateTelegramBotUserRole(_options.BotUserRoleType, botUserId, roleId));
        await _db.SaveChangesAsync();
    }

    public async Task AddUserRole(string username, string role)
    {
        var botUserId = await LinqHelper.GetBotUserId(_users, _botUsers, _botId, username);
        var roleId = (await _db.Set<TRole>().OrderBy(r => r.Id).SingleAsync(r => r.Name == role)).Id;

        await _db.AddAsync(ContextBuilder.CreateTelegramBotUserRole(_options.BotUserRoleType, botUserId, roleId));
        await _db.SaveChangesAsync();
    }

    public async Task RemoveUserRole(long telegramId, string role)
    {
        _db.Remove(await (from r in _roles
            join ur in _userRoles on r.Id equals ur.TelegramRoleId
            join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
            join u in _users on bu.TelegramUserId equals u.Id
            where u.TelegramId == telegramId && r.Name == role && bu.TelegramBotId.Equals(_botId)
            select ur).SingleAsync());

        await _db.SaveChangesAsync();
    }

    public async Task RemoveUserRole(string username, string role)
    {
        _db.Remove(await (from r in _roles
            join ur in _userRoles on r.Id equals ur.TelegramRoleId
            join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
            join u in _users on bu.TelegramUserId equals u.Id
            where u.Username == username && r.Name == role && bu.TelegramBotId.Equals(_botId)
            select ur).SingleAsync());

        await _db.SaveChangesAsync();
    }

    public async Task<List<TRole>> GetUserRoles(long telegramId)
    {
        return await (from r in _roles
            join ur in _userRoles on r.Id equals ur.TelegramRoleId
            join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
            join u in _users on bu.TelegramUserId equals u.Id
            where u.TelegramId == telegramId && bu.TelegramBotId.Equals(_botId)
            select r).Cast<TRole>().ToListAsync();
    }

    public async Task<List<TRole>> GetUserRoles(string username)
    {
        return await (from r in _roles
            join ur in _userRoles on r.Id equals ur.TelegramRoleId
            join bu in _botUsers on ur.TelegramBotUserId equals bu.Id
            join u in _users on bu.TelegramUserId equals u.Id
            where u.Username == username && bu.TelegramBotId.Equals(_botId)
            select r).Cast<TRole>().ToListAsync();
    }
}