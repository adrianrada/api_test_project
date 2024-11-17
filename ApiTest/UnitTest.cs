using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Xunit;
using Xunit.Abstractions;
using Meziantou.Extensions.Logging.Xunit;
using System.Runtime.CompilerServices;

namespace ApiTest
{
    public class UnitTest1
    {

        private readonly ILogger<UnitTest1> _logger_test;
        private readonly ILogger<ApiService> _logger_api;
        private readonly ApiService _apiService;

        public UnitTest1(ITestOutputHelper output)
        {
            // Create a logger that writes to xUnit's output
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddXunit(output);
            });

            _logger_test = loggerFactory.CreateLogger<UnitTest1>();
            _logger_api = loggerFactory.CreateLogger<ApiService>();

            _apiService = new ApiService(_logger_api);
        }

        #region Scenario 1 - Test the retrieval of tasks 

        [Fact]
        [Trait("Category", "Scenario 1 - Test the retrieval of tasks")]
        public async Task Test1GetResponseOk()
        {
            _logger_test.LogInformation("Starting TEST1");
            HttpResponseMessage response = await _apiService.GetAsync("/tasks");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            _logger_test.LogInformation("Ending TEST1");
        }

        [Fact]
        [Trait("Category", "Scenario 1 - Test the retrieval of tasks")]
        public async Task Test2GetResponseJsonFormat()
        {
            HttpResponseMessage response = await _apiService.GetAsync("/tasks");
            var responseFormat = response.Content.Headers.ContentType!;
            Assert.True(responseFormat.MediaType == "application/json",
                $"Response is not in json format, actual response format is {responseFormat.MediaType}");
        }

        [Fact]
        [Trait("Category", "Scenario 1 - Test the retrieval of tasks")]
        public async Task Test3GetEmptyListAfterCleanup()
        {
            List<JObject> taskListBeforeCleanup = await _apiService.GetListAsync("/tasks");

            await _apiService.ClearAllTasksAsync("/tasks", taskListBeforeCleanup);

            List<JObject> taskListAfterCleanup = await _apiService.GetListAsync("/tasks");

            Assert.Empty(taskListAfterCleanup);
        }

        #endregion

        #region Scenario 2 - Test the creation of a new task

        [Theory]
        [Trait("Category", "Scenario 2 - Test the creation of a new task")]
        [InlineData("{\"name\":\"task41\", \"isCompleted\": false}", HttpStatusCode.OK)]
        [InlineData("{\"name\":\"task42\", \"isCompleted\": true}", HttpStatusCode.OK)]
        [InlineData("{\"name\":\"task43\", \"isCompleted\": \"IsFalse\"}", HttpStatusCode.BadRequest)]
        [InlineData("{\"name\":\"\", \"isCompleted\": \"IsFalse\"}", HttpStatusCode.BadRequest)]
        [InlineData("{\"name\":\"tasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktask\",\"isCompleted\": false}", 
            HttpStatusCode.OK)]
        [InlineData("{\"name\":\"tasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktasktask1\",\"isCompleted\": false}",
            HttpStatusCode.BadRequest)]
        public async Task Test4CreateTask(string task, HttpStatusCode expected)
        {
            var response = await _apiService.PostAsync("/tasks", task);

            Assert.Equal(expected, response.StatusCode);
        }

        #endregion

        #region Scenario 3 - Test task update

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [Trait("Category", "Scenario 3 - Test task update")]
        public async Task Test5UpdateExistingTask(string update, bool expected)
        {
            List<JObject> taskList = await _apiService.GetListAsync("/tasks");

            if (taskList.Count == 0)
            {
                await _apiService.PostAsync("/tasks", "{\"name\":\"task51\"}");
                taskList = await _apiService.GetListAsync("/tasks");
            }

            string taskName = (String)taskList[0].Property("name")!.Value!;
            string taskId = (String)taskList[0].Property("id")!.Value!;

            string contentUpdate = "{" + $"\"name\": \"{taskName}\", \"isCompleted\": {update}" + "}";

            await _apiService.PutAsync($"/tasks/{taskId}", contentUpdate);

            taskList = await _apiService.GetListAsync("/tasks");
            bool taskIsCompleted = (bool)taskList[0].Property("isCompleted")!.Value!;
            
            Assert.True(taskIsCompleted == expected);
        }

        [Fact]
        [Trait("Category", "Scenario 3 - Test task update")]
        public async Task Test6UpdateNonExistingTask()
        {
            List<string> taskIdList = [];
            string bogusId;
            string contentUpdate = "{\"name\":\"task61\", \"isCompleted\": false}";

            List<JObject> taskList = await _apiService.GetListAsync("/tasks");

            foreach (JObject task in taskList)
            {
                taskIdList.Add((String)task.Property("id")!.Value!);
            }

            do
            {
                bogusId = Guid.NewGuid().ToString();
            }
            while (taskIdList.Contains(bogusId));

            HttpResponseMessage response = await _apiService.PutAsync($"/tasks/{bogusId}", contentUpdate);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
        #endregion

        #region Scenario 4 - Test task deletion

        [Fact]
        [Trait("Category", "Scenario 4 - Test task deletion")]
        public async Task Test7DeleteExistingTask()
        {
            List<string> taskIdList = [];

            List<JObject> taskList = await _apiService.GetListAsync("/tasks");

            if (taskList.Count == 0)
            {
                await _apiService.PostAsync("/tasks", "{\"name\":\"task71\"}");
                taskList = await _apiService.GetListAsync("/tasks");
            }
            string taskIdFirst = (string)taskList[0].Property("id")!.Value!;

            HttpResponseMessage response = await _apiService.DeleteAsync($"/tasks/{taskIdFirst}");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            taskList = await _apiService.GetListAsync("/tasks");

            foreach (JObject task in taskList)
            {
                taskIdList.Add((string)task.Property("id")!.Value!);
            }
            Assert.DoesNotContain(taskIdFirst, taskIdList);
        }

        [Fact]
        [Trait("Category", "Scenario 4 - Test task deletion")]
        public async Task Test8DeleteNonExistingTask()
        {
            string bogusId;
            List<string> taskIdList = [];
            
            List<JObject> taskList = await _apiService.GetListAsync("/tasks");

            foreach (JObject task in taskList)
            {
                taskIdList.Add((String)task.Property("id")!.Value!);
            }
            do
            {
                bogusId = Guid.NewGuid().ToString();
            }
            while (taskIdList.Contains(bogusId));

            HttpResponseMessage response = await _apiService.DeleteAsync($"/tasks/{bogusId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion
    }
}