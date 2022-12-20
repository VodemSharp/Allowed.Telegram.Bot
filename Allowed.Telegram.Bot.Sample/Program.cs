using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Sample.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connection).UseSnakeCaseNamingConvention(),
    ServiceLifetime.Transient, ServiceLifetime.Transient);

builder.Services.AddTelegramClients(new[]
    {
        // new SimpleTelegramBotClientOptions("<NAME>", "<TOKEN>"),
        new SafeTelegramBotClientOptions("Sample", "<TOKEN>")
    })
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