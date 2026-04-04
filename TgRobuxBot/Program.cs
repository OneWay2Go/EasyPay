using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TgRobuxBot.Models;
using TgRobuxBot.Services;

var builder = Host.CreateApplicationBuilder(args);

// Ensure user secrets are loaded (optional, already added in Host.CreateApplicationBuilder in dev)
builder.Configuration.AddUserSecrets<Program>();

// Add DbContext
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var botToken = builder.Configuration["BotToken"];
if (string.IsNullOrEmpty(botToken))
{
    throw new Exception("BotToken is missing in configuration/user-secrets!");
}

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

builder.Services.AddSingleton<UpdateHandler>();
builder.Services.AddHostedService<BotBackgroundService>();

var host = builder.Build();
host.Run();
