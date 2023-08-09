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
using System.Text;
using System.Threading.Tasks;
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

        public async Task<LinkedList<Member>> GetSettelitesData()
        {
            int pageSize = _options.PageSize;
            int batchSize = _options.ParallelRequestsBatchSize;

            var itemsCount = await GetItemsCount();

            var itemsList = new LinkedList<Member>();

            int pageCount = (int)Math.Ceiling((double)itemsCount / pageSize);


            int batchCount = (int)Math.Ceiling((double)pageCount / batchSize);

            _logger?.LogInformation("Downloading started");

            for (int i = 1; i <= pageCount; i += batchSize)
            {
                if (i + batchSize > pageCount)
                    batchSize = pageCount - i + 1;

                var items = await GetPagesParalelly(i, i + batchSize - 1, 100);

                _logger?.LogInformation($"{ ((double)(i + batchSize - 1) / pageCount).ToString("0.00%") } Done");                

                itemsList.Concat(items);
            }                           

            return itemsList;
        }

        private async Task<Member[]> GetPagesParalelly(int pageStart, int pageEnd, int pageSize)
        {
            var pageCount = pageEnd - pageStart + 1;

            var tasks = new List<Task<IEnumerable<Member>>>(pageCount);

            for (int i = pageStart; i <= pageEnd; i++)
            {
                tasks.Add(GetPageOfitems(i, pageSize));
            }

            _logger?.LogInformation($"{tasks.Count} requests started");

            var responses = await Task.WhenAll(tasks);

            var dataItems = responses.SelectMany(x => x);

            return dataItems.ToArray();
        }

        private async Task<IEnumerable<Member>> GetPageOfitems(int pageNumber, int pageSize)
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", pageSize.ToString());
            queryParameters.Add("page", pageNumber.ToString());

            var response = await GetAsync<SetelliteServiceResponse>(_url, queryParameters);
            return response.ResponseDto.Member;
        }

        private async Task<int> GetItemsCount()
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", "1");           

            var response = await GetAsync<SetelliteServiceResponse>(_url, queryParameters);
            return response.ResponseDto.TotalItems;
        }
    }
}
