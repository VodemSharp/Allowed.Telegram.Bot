using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection),
    ServiceLifetime.Transient, ServiceLifetime.Transient);

builder.Services.AddTelegramClients(builder.Configuration.GetSection("Telegram:Bots").Get<BotData[]>())
    .AddTelegramStore<ApplicationDbContext>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTelegramDbManager();
else
    builder.Services.AddTelegramDbWebHookManager();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();