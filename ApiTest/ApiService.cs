﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace ApiTest
{
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
                //BaseAddress = new Uri("https://localhost:7246/tasks")
                BaseAddress = new Uri($"{url}")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task ClearAllTasksAsync(string endpoint, List<JObject> taskList)
        {

            foreach (JObject task in taskList)
            {
                var itemId = (string)task.Property("id")!.Value!;

                _logger.LogInformation($"DELETE task id {itemId}");
                await _httpClient.DeleteAsync($"{endpoint}/{itemId}");
            }
        }

        // Method to perform a GET request that returns response as list of JSON objects
        public async Task<List<JObject>> GetListAsync(string endpoint)
        {
            _logger.LogInformation($"GET resources from {endpoint} as list of JObjects");
            string responseDefault = await _httpClient.GetStringAsync(endpoint);

            return JsonConvert.DeserializeObject<List<JObject>>(responseDefault)!;
        }

        // Method to perform a POST request
        public async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"POST {jsonContent} to endpoint {endpoint}");
            return await _httpClient.PostAsync(endpoint, content);
        }

        /// <summary>
        /// Send PUT to <see cref="endpoint"/> endpoint
        /// </summary>
        /// <param name="endpoint">endpoint to check</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> PutAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"PUT {jsonContent} to endpoint {endpoint}");
            return await _httpClient.PutAsync(endpoint, content);
        }

        /// <summary>
        /// Send GET to <see cref="endpoint"/> endpoint
        /// </summary>
        /// <param name="endpoint">endpoint to check</param>
        /// <returns>HttpResponseMessage</returns>
        public new async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            _logger.LogInformation($"GET resources from endpoint {endpoint}");
            return await _httpClient.GetAsync(endpoint);
        }

        /// <summary>
        /// Send DELETE to <see cref="endpoint"/> to delete specific resource. Will need to add the resource id to the endpoint uri.
        /// </summary>
        /// <param name="endpoint">endpoint delete</param>
        /// <returns>HttpResponseMessage</returns>
        public new async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            _logger.LogInformation($"DELETE endpoint {endpoint}");
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}
