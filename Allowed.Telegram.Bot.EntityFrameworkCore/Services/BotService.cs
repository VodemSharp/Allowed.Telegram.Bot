using Allowed.Telegram.Bot.Data.Entities;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services;

public class BotService<TContext, TBot>(TContext db) : IBotService
    where TContext : DbContext
    where TBot : TelegramBot, new()
{
    private readonly DbSet<TBot> _bots = db.Set<TBot>();
    
    public async Task Add(long id)
    {
        await _bots.AddAsync(new TBot { Id = id });
        await db.SaveChangesAsync();
    }

    public async Task<bool> Any(long id)
    {
        return await _bots.AnyAsync(x => x.Id == id);
    }
}