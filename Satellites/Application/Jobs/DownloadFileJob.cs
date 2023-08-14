using Microsoft.Extensions.Logging;
using Satellites.Helpers;
using Satellites.Services.SettelitesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Satellites.Application.Jobs
{
    public class DownloadFileJob : JobBase
    {
        private readonly SatellitesService _satelliteService;

        public DownloadFileJob(SatellitesService satellitesService, string name, ApplictationSettings applictationSettings, ILogger logger) : base(name, applictationSettings, logger)
        {
            _satelliteService = satellitesService;
        }

        protected override async Task JobLogic()
        {
            var data = await _satelliteService.GetSettelitesData();

            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(jsonString);
          
            await File.WriteAllBytesAsync(_applicationSettings.DownloadaPath, inputBytes);            
        }
    }
}
