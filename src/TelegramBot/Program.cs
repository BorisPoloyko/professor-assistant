using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using TelegramBot.Model.Configurations;
using TelegramBot.Services.Implementations.Requests;
using TelegramBot.Services.Implementations.Webhook;
using TelegramBot.Services.Interfaces.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("/app/logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection(BotConfiguration.Configuration));
builder.Services.Configure<AccessConfiguration>(builder.Configuration.GetSection(AccessConfiguration.Configuration));

builder.Services
    .AddHttpClient("telegramBot")
    .AddTypedClient<ITelegramBotClient>((client, provider) =>
{
    var botConfiguration = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    var botClientOptions = new TelegramBotClientOptions(botConfiguration.BotToken);

    return new TelegramBotClient(botClientOptions, client);
});
builder.Services.AddTransient<IRequestFactory, RequestFactory>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

app.MapControllers();

app.Run();