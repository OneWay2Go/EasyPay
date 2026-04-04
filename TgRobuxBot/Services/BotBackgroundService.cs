using Telegram.Bot;

namespace TgRobuxBot.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandler _updateHandler;
    private readonly ILogger<BotBackgroundService> _logger;

    public BotBackgroundService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<BotBackgroundService> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bot is starting receiving updates...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _botClient.ReceiveAsync(
                    updateHandler: _updateHandler.HandleUpdateAsync,
                    errorHandler: _updateHandler.HandlePollingErrorAsync,
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while receiving updates");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
