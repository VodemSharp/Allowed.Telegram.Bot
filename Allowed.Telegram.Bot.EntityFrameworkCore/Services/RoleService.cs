using Allowed.Telegram.Bot.Data.Entities;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services;

public class RoleService<TContext, TKey, TRole, TBotUserRole>(TContext db) : IRoleService
    where TContext : DbContext
    where TKey : IEquatable<TKey>
    where TRole : TelegramRole<TKey>, new()
    where TBotUserRole : TelegramBotUserRole<TKey>, new()
{
    private readonly DbSet<TRole> _roles = db.Set<TRole>();
    private readonly DbSet<TBotUserRole> _userRoles = db.Set<TBotUserRole>();

    #region Roles

    public async Task<bool> Any(string role)
    {
        return await _roles.AnyAsync(r => r.Name == role);
    }

    public async Task Add(string role)
    {
        await _roles.AddAsync(new TRole { Name = role });
        await db.SaveChangesAsync();
    }

    public async Task Remove(string role)
    {
        _roles.Remove(await _roles.SingleAsync(x => x.Name == role));
        await db.SaveChangesAsync();
    }

    #endregion

    #region BotUserRoles

    public async Task<bool> Any(long botId, long userId, string roleName)
    {
        return await _userRoles
            .Join(_roles, userRole => userRole.TelegramRoleId, role => role.Id,
                (userRole, role) => new
                {
                    userRole.TelegramBotId,
                    userRole.TelegramUserId,
                    RoleName = role.Name
                })
            .AnyAsync(x => x.TelegramBotId == botId && x.TelegramUserId == userId && x.RoleName == roleName);
    }

    public async Task Add(long botId, long userId, string roleName)
    {
        var role = await _roles.SingleAsync(x => x.Name == roleName);

        await _userRoles.AddAsync(new TBotUserRole
        {
            TelegramBotId = botId,
            TelegramUserId = userId,
            TelegramRoleId = role.Id
        });

        await db.SaveChangesAsync();
    }

    public async Task Remove(long botId, long userId, string roleName)
    {
        var userRole = await _userRoles
            .Join(_roles, userRole => userRole.TelegramRoleId, role => role.Id,
                (userRole, role) => new { UserRole = userRole, RoleName = role.Name })
            .Where(x => x.UserRole.TelegramBotId == botId
                        && x.UserRole.TelegramUserId == userId
                        && x.RoleName == roleName)
            .Select(x => x.UserRole)
            .SingleOrDefaultAsync();

        if (userRole != null)
        {
            _userRoles.Remove(userRole);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<string>> Get(long botId, long userId)
    {
        return await _userRoles
            .Where(userRole => userRole.TelegramBotId == botId && userRole.TelegramUserId == userId)
            .Join(_roles, userRole => userRole.TelegramRoleId, role => role.Id, (userRole, role) => role.Name)
            .ToListAsync();
    }

    #endregion
}