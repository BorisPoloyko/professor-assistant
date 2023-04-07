using System.Net;
using System.Text.Json;
using TelegramBot.Model.Clients;

namespace TelegramBot.Services.Implementations.HttpClients
{
    public class StudentsClient
    {
        private readonly HttpClient _httpClient;

        public StudentsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StudentInfoDto?> GetStudentInfo(long id)
        {
            using var response = await _httpClient.GetAsync($"/api/students/{id}");
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StudentInfoDto>(resultString, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        }

        public async Task CreateOrUpdateStudent(StudentDto student)
        {
            using var response = await _httpClient.GetAsync($"/api/students/{student.Id}");
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                {
                    using var request = await _httpClient.PostAsync("/api/students", JsonContent.Create(student));
                    request.EnsureSuccessStatusCode();
                    break;
                }
                case HttpStatusCode.OK:
                {
                    using var request = await _httpClient.PutAsync($"/api/students/{student.Id}", JsonContent.Create(student));
                    request.EnsureSuccessStatusCode();
                    break;
                }
            }
        }

        public async Task UpdateStudent(StudentDto student)
        {
            var request = await _httpClient.PutAsync($"/api/students/{student.Id}", JsonContent.Create(new
            {
                student.FirstName,
                student.LastName
            }));
            request.EnsureSuccessStatusCode();
        }
    }
}
