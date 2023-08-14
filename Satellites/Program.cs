// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Satellites.Application;
using Satellites.Application.Jobs;
using Satellites.Helpers;
using Satellites.Services.SetellitesService;
using Satellites.Services.SettelitesService;

var configBuilder = new ConfigurationBuilder();
configBuilder.Set();
IConfiguration config = configBuilder.Build();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger<SatellitesService>();



//var watch = new System.Diagnostics.Stopwatch();

//watch.Start();

//Console.WriteLine(items.Count());

//watch.Stop();

//Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

var app = new Application();

var applicationSettings = config.GetSection("applicationSettings").Get<ApplictationSettings>();
var setelliteServiceOptions = config.GetSection("satelliteServiceOptions").Get<SatelliteServiceOptions>();

using var satelliteService = new SatellitesService(setelliteServiceOptions, new HttpClient(), logger);

app.Jobs.Add(new DownloadFileJob(satelliteService, "Download satellites data", applicationSettings, logger));

app.Jobs.Add(new LocalSatelliteInformationCheckJob("Check locale data available", applicationSettings, logger));

app.Jobs.Add(new CompareHashSumJob(satelliteService, "Check locale data available", applicationSettings, logger));

app.Run().RunSynchronously();

