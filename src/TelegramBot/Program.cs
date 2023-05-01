using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using TelegramBot.Middlewares;
using TelegramBot.Model.Configurations;
using TelegramBot.Services.Implementations.BackgroundServices;
using TelegramBot.Services.Implementations.Dialogs;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using StartupService = TelegramBot.Services.Implementations.Webhook.StartupService;

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


var identityBaseAddress = new Uri(builder.Configuration["Clients:Identity:BaseUrl"]);


void IdentityClientOptions(HttpClient client)
{
    client.BaseAddress = identityBaseAddress;
}

var studentsClient = builder.Services.AddHttpClient<StudentsClient>(IdentityClientOptions);
var groupsClient = builder.Services.AddHttpClient<UniversityGroupsClient>(IdentityClientOptions);

Action<HttpMessageHandlerBuilder> IdentityClientHandlerBuilder()
{
    return handlerBuilder =>
    {
        handlerBuilder.PrimaryHandler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    };
}

if (builder.Environment.IsDevelopment())
{
    studentsClient.ConfigureHttpMessageHandlerBuilder(IdentityClientHandlerBuilder());
    groupsClient.ConfigureHttpMessageHandlerBuilder(IdentityClientHandlerBuilder());
}

builder.Services.AddSingleton(typeof(CacheSignal<>));
builder.Services.AddTransient<IDialogFactory, DialogFactory>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHostedService<StartupService>();
builder.Services.AddTransient<IUniversityGroupsService, UniversityGroupsService>();
builder.Services.AddHostedService<UniversityGroupCacheService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();