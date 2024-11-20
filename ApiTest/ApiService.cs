namespace ApiTest
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection.Metadata;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RandomString4Net;

    public class ApiService : HttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(string url, ILogger<ApiService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10),
                
                // BaseAddress = new Uri("https://localhost:7246/tasks")
                BaseAddress = new Uri($"{url}"),
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Delete all tasks from <see cref="endpoint"/> endpoint.
        /// </summary>
        /// <param name="endpoint">endpoint to check.</param>
        /// <param name="taskList"> list of existing tasks obtainable using GetListAsync().</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task ClearAllTasksAsync(string endpoint, List<JObject> taskList)
        {
            foreach (JObject task in taskList)
            {
                var itemId = (string)task.Property("id")!.Value!;

                _logger.LogInformation($"DELETE task id {itemId}");
                await _httpClient.DeleteAsync($"{endpoint}/{itemId}");
            }
        }

        /// <summary>
        /// Send GET to <see cref="endpoint"/> endpoint and retrieve the tasks as list of JObjects.
        /// </summary>
        /// <param name="endpoint">endpoint to check.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<List<JObject>> GetListAsync(string endpoint)
        {
            _logger.LogInformation($"GET resources from {endpoint} as list of JObjects");
            string responseDefault = await _httpClient.GetStringAsync(endpoint);

            return JsonConvert.DeserializeObject<List<JObject>>(responseDefault)!;
        }

        /// <summary>
        /// Send POST to <see cref="endpoint"/> endpoint.
        /// </summary>
        /// <param name="endpoint">endpoint to check.</param>
        /// <param name="jsonContent">json content as string to be sent to server.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"POST {jsonContent} to endpoint {endpoint}");
            return await _httpClient.PostAsync(endpoint, content);
        }

        /// <summary>
        /// Send PUT to <see cref="endpoint"/> endpoint.
        /// </summary>
        /// <param name="endpoint">endpoint to check.</param>
        /// <param name="jsonContent">json content as string to be sent to server.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> PutAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"PUT {jsonContent} to endpoint {endpoint}");
            return await _httpClient.PutAsync(endpoint, content);
        }

        /// <summary>
        /// Send GET to <see cref="endpoint"/> endpoint.
        /// </summary>
        /// <param name="endpoint">endpoint to check.</param>
        /// <returns>HttpResponseMessage.</returns>
        public new async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            _logger.LogInformation($"GET resources from endpoint {endpoint}");
            return await _httpClient.GetAsync(endpoint);
        }

        /// <summary>
        /// Send DELETE to <see cref="endpoint"/> to delete specific resource. Will need to add the resource id to the endpoint uri.
        /// </summary>
        /// <param name="endpoint">endpoint delete.</param>
        /// <returns>HttpResponseMessage.</returns>
        public new async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            _logger.LogInformation($"DELETE endpoint {endpoint}");
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}
