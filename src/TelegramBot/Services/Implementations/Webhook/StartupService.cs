using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Model.Configurations;

namespace TelegramBot.Services.Implementations.Webhook
{
    public class StartupService : IHostedService
    {
        private readonly ILogger<StartupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BotConfiguration _botConfigurations;

        public StartupService(ILogger<StartupService> logger, IOptions<BotConfiguration> botConfigurations, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfigurations = botConfigurations.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            _logger.LogWarning(_botConfigurations.WebhookUrl);
            await botClient.SetWebhookAsync(_botConfigurations.WebhookUrl, cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
