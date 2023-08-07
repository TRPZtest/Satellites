using Microsoft.Extensions.Logging;
using Satellites.Helpers;
using Satellites.Services.ServiceRest;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public SatellitesService(string serviceUrl, HttpClient httpClient, ILogger? logger = null) : base(httpClient, logger) 
        {
            _url = serviceUrl;    
        }

        //public async Task<IEnumerable<Member>> GetSettelitesData()
        //{
        //    const int pageSize = 100;

        //    var queryParameters = new NameValueCollection();
        //    queryParameters.Add("page-size", "1");

        //    var queryString = queryParameters.ToQueryString(true);

        //    var response = await GetAsync<SetelliteServiceResponse>(_url + queryString);

        //    return response.TotalItems
        //}

        public async Task<int> GetItemsNumber()
        {
            var queryParameters = new NameValueCollection();
            queryParameters.Add("page-size", "1");

            var queryString = queryParameters.ToQueryString(true);

            var response = await GetAsync<SetelliteServiceResponse>(_url + queryString);
            return response.TotalItems;
        }

    }
}
