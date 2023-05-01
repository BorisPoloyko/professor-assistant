using Microsoft.Extensions.Caching.Memory;
using TelegramBot.Services.Implementations.HttpClients;

namespace TelegramBot.Services.Implementations.BackgroundServices
{
    public class UniversityGroupCacheService : BackgroundService
    {
        private readonly IMemoryCache _cache;
        private readonly UniversityGroupsClient _client;
        private readonly CacheSignal<UniversityGroupsService> _cacheSignal;
        private bool _isInitialized;
        public UniversityGroupCacheService(IMemoryCache cache, UniversityGroupsClient client, CacheSignal<UniversityGroupsService> cacheSignal)
        {
            _cache = cache;
            _client = client;
            _cacheSignal = cacheSignal;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cacheSignal.WaitAsync();
            await base.StartAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var groups = await _client.GetAllStudentGroups();

                    _cache.Set("uni_groups", groups?.ToArray());
                }
                finally
                {
                    if (!_isInitialized)
                    {
                        _cacheSignal.Release();
                        _isInitialized = true;
                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
                catch (OperationCanceledException e)
                {
                    break;
                }
            }
        }
    }
}
