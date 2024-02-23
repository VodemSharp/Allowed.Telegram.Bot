using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Factories;
using Allowed.Telegram.Bot.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTelegramControllers();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTelegramManager();
else
    builder.Services.AddTelegramWebHookManager();

var app = builder.Build();

var telegramManager = app.Services.GetRequiredService<ITelegramManager>();
telegramManager.Start(new[]
{
    // TelegramBotClientFactory.CreateClient(new SimpleTelegramBotClientOptions("<NAME>", "<TOKEN>")),
    TelegramBotClientFactory.CreateClient(new SafeTelegramBotClientOptions("<NAME>", "<TOKEN>"))
});

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