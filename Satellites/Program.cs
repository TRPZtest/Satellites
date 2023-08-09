// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Satellites.Helpers;
using Satellites.Services.SetellitesService;
using Satellites.Services.SettelitesService;

var configBuilder = new ConfigurationBuilder();

configBuilder.Set();

IConfiguration config = configBuilder.Build();

var setelliteServiceOptions = config.GetSection("SatelliteServiceOptions").Get<SatelliteServiceOptions>();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger<SatellitesService>();

var watch = new System.Diagnostics.Stopwatch();

watch.Start();

using var satelliteService = new SatellitesService(setelliteServiceOptions, new HttpClient(), logger);

var items = await satelliteService.GetSettelitesData();

Console.WriteLine(items.Count());

watch.Stop();

Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");