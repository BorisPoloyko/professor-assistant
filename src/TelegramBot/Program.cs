using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Model.Configurations;
using TelegramBot.Services.Webhook;

var builder = WebApplication.CreateBuilder(args);

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

builder.Services
    .AddHttpClient("telegramBot")
    .AddTypedClient<ITelegramBotClient>((client, provider) =>
{
    var botConfiguration = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    var botClientOptions = new TelegramBotClientOptions(botConfiguration.BotToken);

    return new TelegramBotClient(botClientOptions, client);
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

app.MapControllers();

app.Run();