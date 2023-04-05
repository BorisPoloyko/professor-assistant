using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using TelegramBot.Middlewares;
using TelegramBot.Model.Configurations;
using TelegramBot.Services.Implementations.Dialogs;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Implementations.Requests;
using TelegramBot.Services.Implementations.Webhook;
using TelegramBot.Services.Interfaces.Dialogs;
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


var studentsClient = builder.Services.AddHttpClient<StudentsClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Clients:Identity:BaseUrl"]);
});

if (builder.Environment.IsDevelopment())
{
    studentsClient.ConfigureHttpMessageHandlerBuilder(handlerBuilder =>
    {
        handlerBuilder.PrimaryHandler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    });
}

builder.Services.AddTransient<IDialogFactory, DialogFactory>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHostedService<StartupService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();