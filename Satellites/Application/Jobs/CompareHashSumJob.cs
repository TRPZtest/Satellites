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
    public class CompareHashSumJob : JobBase
    {
        private readonly SatellitesService _satelliteService;

        public CompareHashSumJob(SatellitesService satellitesService, string name, ApplictationSettings applictationSettings, ILogger logger) : base(name, applictationSettings, logger)
        {
            _satelliteService = satellitesService;
        }

        protected override async Task JobLogic()
        {

            var data = await _satelliteService.GetSettelitesData();

            var jsonString = JsonSerializer.Serialize(data);

            byte[] freshDataBytes = System.Text.Encoding.ASCII.GetBytes(jsonString);

            freshDataBytes.GetMD5();


            var fileBytes = await File.ReadAllBytesAsync(_applicationSettings.DownloadaPath);

            var localFileHashSum = fileBytes.GetMD5();

            _logger.LogInformation($"Local data hash: {localFileHashSum}, API data hash: {freshDataBytes}");
        }
    }
}
