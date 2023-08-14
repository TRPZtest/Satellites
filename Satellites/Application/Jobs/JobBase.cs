using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satellites.Application.Jobs
{
    public abstract class JobBase
    {
        protected readonly ApplictationSettings _applicationSettings;
        protected readonly ILogger _logger;
        public string Name { get; protected set; }
        protected ILogger? Logger { get; set; }           

        protected JobBase(string name, ApplictationSettings applictationSettings, ILogger logger)
        {
            _applicationSettings = applictationSettings;
            _logger = logger;
            Name = name;
        }

        protected abstract Task JobLogic();

        public async Task ExecuteJobAsync()
        {
            try
            {               
                await JobLogic();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Message);
            }
        }
    }
}
