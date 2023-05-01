using Microsoft.Extensions.Caching.Memory;
using TelegramBot.Model.Clients;

namespace TelegramBot.Services.Implementations.HttpClients
{
    public class UniversityGroupsService : IUniversityGroupsService
    {
        private readonly CacheSignal<UniversityGroupsService> _cacheSignal;
        private readonly UniversityGroupsClient _client;
        private readonly IMemoryCache _cache;

        public UniversityGroupsService(CacheSignal<UniversityGroupsService> cacheSignal, IMemoryCache cache, UniversityGroupsClient client)
        {
            _cacheSignal = cacheSignal;
            _cache = cache;
            _client = client;
        }

        public async Task<IEnumerable<UniversityGroupDto?>> GetUniversityGroups(Func<UniversityGroupDto, bool>? filter)
        {
            try
            {
                // will be released after ExecuteAsync in BackgroundService
                await _cacheSignal.WaitAsync();

                var groups = await _cache.GetOrCreateAsync("uni_groups",
                    _ => Task.FromResult(Array.Empty<UniversityGroupDto>()));

                filter ??= _ => true;

                return groups.Where(filter).ToArray();
            }
            finally
            {
                _cacheSignal.Release();
            }
        }

        public Task EnrollStudentToGroup(long studentId, int groupId) =>
            _client.EnrollStudentToGroup(studentId, groupId);
    }
}
