using Azure.Core.Diagnostics;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Serilog;
using System.Security.Principal;
using Telegram.Bot;
using TelegramBot.Middlewares;
using TelegramBot.Model.Configurations;
using TelegramBot.Services.Implementations.BackgroundServices;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Implementations.Dialogs;
using TelegramBot.Services.Implementations.Processing;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Processing;

var builder = WebApplication.CreateBuilder(args);
using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

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

builder.Services.AddSingleton(_ =>
{
    var options = new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway };
    return new CosmosClient(builder.Configuration["Cosmos:ConnectionString"], options);
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(new Uri(builder.Configuration["BlobClient:Uri"]));
    clientBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = builder.Configuration["BlobClient:ManagedIdentityId"],
        TenantId = builder.Configuration["BlobClient:TenantId"]
    }));
});

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSingleton(typeof(CacheSignal<>));
builder.Services.AddTransient<IDialogProvider, DialogProvider>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHostedService<StartupService>();
builder.Services.AddTransient<IUniversityGroupsService, UniversityGroupsService>();
builder.Services.AddTransient<IAssignmentsClient, AssignmentsClients>();
builder.Services.AddTransient<IUpdateProcessor, UpdateProcessor>();
builder.Services.AddHostedService<UniversityGroupCacheService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();