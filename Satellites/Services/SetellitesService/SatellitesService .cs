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

        public async Task<List<Member>> GetSettelitesData()
        {          
            int pageSize = _options.PageSize;
            int batchSize = _options.ParallelRequestsBatchSize;
         
            var itemsCount = await GetItemsCount();

            var itemsList = new List<Member>();

            int pageCount = (int)Math.Ceiling((double)itemsCount / pageSize);
           
            int batchCount = (int)Math.Ceiling((double)pageCount / batchSize);      

            var tasks = new List<Task<Member[]>>(pageCount);

            for (int i = 1; i <= pageCount; i++)
            {
                tasks.Add(GetPageOfitems(i, pageSize));
            }

            var tasksBatches = tasks.Chunk(batchSize);

            int itemsDownloaded = 0;

            _logger?.LogInformation("Downloading started");

            foreach (var batch in tasksBatches)
            {
                var responses = await Task.WhenAll(tasks);

                var dataItems = responses.SelectMany(x => x);

                itemsList.AddRange(dataItems);

                itemsDownloaded += dataItems.Count();

                LogProgress(itemsDownloaded, pageCount);                
            }                                             

            return itemsList;
        }

        private async Task<Member[]> GetPageOfitems(int pageNumber, int pageSize)
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", pageSize.ToString());
            queryParameters.Add("page", pageNumber.ToString());

            for (int i = 0; i < _options.RequestRetriesNumber; i ++)
            {
                try
                {
                    var response = await GetAsyncDecorator(_url, queryParameters);
                    return response.Member;
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex.Message);
                }
                Thread.Sleep((_options.RequestRetryDelay));
            }
            
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
