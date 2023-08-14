using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satellites.Services.SetellitesService
{
    public class SatelliteServiceOptions
    {
        public string Url { get; set; }
        public int PageSize { get; set; }
        public int ParallelRequestsBatchSize { get; set; }
        public int RequestRetriesNumber {  get; set; }
        public int RequestRetryDelay { get; set;  }
    }
}
