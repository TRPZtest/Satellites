using Microsoft.Extensions.Logging;
using Satellites.Services.SettelitesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Satellites.Application.Jobs
{
    internal class LocalSatelliteInformationCheckJob : JobBase
    {
        public LocalSatelliteInformationCheckJob(string name, ApplictationSettings applictationSettings, ILogger logger) : base(name, applictationSettings, logger)
        {
        }

        protected override async Task JobLogic()
        {
            int itemsCount = 0;
            try
            {
                using FileStream fileStream = new(_applicationSettings.DownloadaPath, FileMode.Open);
                var file = await JsonSerializer.DeserializeAsync<IEnumerable<Member>>(fileStream);
                itemsCount = file.Count();
            }
            catch (Exception ex)
            {            
                _logger.LogError("Local data reading error:\n" + ex.Message);                   
            }
            _logger.LogInformation($"Local data avaible, {itemsCount} data records saved localy.");
        }
    }
}
