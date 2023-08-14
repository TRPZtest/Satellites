using Microsoft.Extensions.Logging;
using Satellites.Helpers;
using Satellites.Services.ServiceRest;
using Satellites.Services.SetellitesService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;

namespace Satellites.Services.SettelitesService
{
    public class SatellitesService : ServiceREST
    {               
        private readonly string _url;
        private readonly SatelliteServiceOptions _options;
        private readonly ILogger? _logger;

        public SatellitesService(SatelliteServiceOptions options, HttpClient httpClient, ILogger? logger = null) : base(httpClient, logger) 
        {
            _url = options.Url;    
            _options = options;           
            _logger = logger;
        }

        public async Task<Member[]> GetSettelitesData()
        {          
            int pageSize = _options.PageSize;
            int batchSize = _options.ParallelRequestsBatchSize;
         
            var itemsCount = await GetItemsCount(); 

            int pageCount = (int)Math.Ceiling((double)itemsCount / pageSize);

            //pageCount = 4;
           
            int batchCount = (int)Math.Ceiling((double)pageCount / batchSize);      

            var tasks = new List<Task<Member[]>>(pageCount);

            _logger?.LogInformation("Downloading started");
            using var semaphoreSlim = new SemaphoreSlim(batchSize, batchSize);
            for (int i = 1; i <= pageCount; i++)
            {
                tasks.Add(GetPageOfitems(i, pageSize, pageCount, semaphoreSlim));
            }

            var responses = Task.WhenAll(tasks).Result;

            var dataItems = responses.SelectMany(x => x);

            return dataItems.ToArray();
        }

        private async Task<Member[]> GetPageOfitems(int pageNumber, int pageSize, int pageCount, SemaphoreSlim semaphoreSlim)
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", pageSize.ToString());
            queryParameters.Add("page", pageNumber.ToString());
            await semaphoreSlim.WaitAsync();
            for (int i = 0; i < _options.RequestRetriesNumber; i ++)
            {
                try
                {
                    var response = await GetAsyncDecorator(_url, queryParameters);
                  
                    semaphoreSlim.Release();

                    LogProgress(pageNumber, pageCount);
                    return response.Member;
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex.Message);
                }
                Thread.Sleep((_options.RequestRetryDelay));
            }
            semaphoreSlim.Release();
            throw new Exception("Attempts to resend the request have been exhausted.");
        }

        public async Task<int> GetItemsCount()
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", "1");           

            var response = await GetAsyncDecorator(_url, queryParameters);
            
            return response.TotalItems;
        }

        private async Task<SetelliteServiceResponse> GetAsyncDecorator(string url, NameValueCollection queryParameters)
        {            
            var response = await GetAsync<SetelliteServiceResponse>(url + queryParameters.ToQueryString(true));

            return response;         
        }

        private void LogProgress(int itemsDownloaded, int total) => _logger?.LogInformation($"{((double)itemsDownloaded / total).ToString("0.00%")} Done");
    }
}
