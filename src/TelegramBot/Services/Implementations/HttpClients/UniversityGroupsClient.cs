using System.Text.Json;
using TelegramBot.Model.Clients;

namespace TelegramBot.Services.Implementations.HttpClients
{
    public class UniversityGroupsClient
    {
        private readonly HttpClient _httpClient;

        public UniversityGroupsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UniversityGroupDto?>?> GetAllStudentGroups()
        {
            using var response = await _httpClient.GetAsync("/api/groups");
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<UniversityGroupDto?>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task EnrollStudentToGroup(long studentId, int groupId)
        {
            using var response = await _httpClient.PutAsync($"/api/students/{studentId}/enroll/{groupId}", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
