using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTelegramClients(builder.Configuration.GetSection("Telegram:Bots").Get<BotData[]>());

if (builder.Environment.IsDevelopment())
    builder.Services.AddTelegramManager();
else
    builder.Services.AddTelegramWebHookManager();

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