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
        int count = 0;

        public ServiceREST(HttpClient httpClient) 
        {
            _httpClient = httpClient;         
        }

        public ServiceREST(HttpClient httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;

        }

        protected async Task<ResponseWrapper<T>> GetAsync<T>(string url, NameValueCollection queryParameters)
        {
            return await GetAsync<T>(url + queryParameters.ToQueryString(true));
        }

        protected async Task<ResponseWrapper<T>> GetAsync<T>(string url)
        {   
            Console.WriteLine("Get started " + count++);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();

            var  responseObj = new ResponseWrapper<T>() { ResponseDto = JsonSerializer.Deserialize<T>(contentString), IsSuccesStatusCode = response.IsSuccessStatusCode };

            return responseObj;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private void checkHttpCode(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception($"Http code { responseMessage.StatusCode }");
        }
    }
}
