using Microsoft.Extensions.Logging;
using Satellites.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Satellites.Services.ServiceRest
{
    public class ServiceREST : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _logger;         

        public ServiceREST(HttpClient httpClient) 
        {
            _httpClient = httpClient;         
        }

        public ServiceREST(HttpClient httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
     
        protected async Task<T> GetAsync<T>(string url)
        {      
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();

            var  responseObj = JsonSerializer.Deserialize<T>(contentString);

            return responseObj;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }      
    }
}
