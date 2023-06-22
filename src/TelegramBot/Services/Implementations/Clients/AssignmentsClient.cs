using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.Cosmos;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Interfaces.Clients;

namespace TelegramBot.Services.Implementations.Clients
{
    public class AssignmentsClients : IAssignmentsClient
    {
        private readonly CosmosClient _client;
        private readonly StudentsClient _studentsClient;
        public AssignmentsClients(CosmosClient client, StudentsClient studentsClient)
        {
            _client = client;
            _studentsClient = studentsClient;
        }

        public async Task<IEnumerable<AssignmentDto>?> GetAssignmentsByStudentId(long studentId)
        {
            var studentInfo = await _studentsClient.GetStudentInfo(studentId);
            if (studentInfo?.Group?.Course == null || studentInfo.Group?.Group == null)
            {
                return null;
            }
            var query = new QueryDefinition(
            @"select * from c where c.course = @course and c[""group""] = @group").WithParameter("@course", studentInfo.Group.Course).WithParameter("@group", studentInfo.Group.Group);
            var container = _client.GetContainer("ProfessorAssistant", "Assignments");
            var feedIterator = container.GetItemQueryIterator<AssignmentDto>(query);
            var assignments = new List<AssignmentDto>();
            while (feedIterator.HasMoreResults)
            {
                assignments.AddRange(await feedIterator.ReadNextAsync());

            }

            return assignments;
        }

        public async Task<AssignmentDto?> GetAssignmentById(long id)
        {
            var query = new QueryDefinition(
                @"SELECT * FROM c WHERE c[""id""] = @id").WithParameter("@id", id.ToString());
            var container = _client.GetContainer("ProfessorAssistant", "Assignments");
            var feedIterator = container.GetItemQueryIterator<AssignmentDto>(query);
            while (feedIterator.HasMoreResults)
            {
                var result = await feedIterator.ReadNextAsync();
                return result.FirstOrDefault();

            }

            return null;
        }


        public async Task<IEnumerable<AssignmentDto>?> GetActiveAssignments(long studentId)
        {
            var studentInfo = await _studentsClient.GetStudentInfo(studentId);
            if (studentInfo?.Group?.Course == null || studentInfo.Group?.Group == null)
            {
                return null;
            }
            var query = new QueryDefinition(
                @"select * from c where c.startDate < @now and c.endDate > @now and c.course = @course and c[""group""] = @group").WithParameter("@now", DateTime.UtcNow).WithParameter("@course", studentInfo.Group.Course).WithParameter("@group", studentInfo.Group.Group);
            var container = _client.GetContainer("ProfessorAssistant", "Assignments");
            var feedIterator = container.GetItemQueryIterator<AssignmentDto>(query);
            var assignments = new List<AssignmentDto>();
            while (feedIterator.HasMoreResults)
            {
                assignments.AddRange(await feedIterator.ReadNextAsync());

            }

            return assignments;
        }
    }
}
