// See https://aka.ms/new-console-template for more information
using Satellites.Helpers;
using Satellites.Services.SettelitesService;

var satelliteService = new SatellitesService("https://tle.ivanstanojevic.me/api/tle", new HttpClient());

var itemsCount = await satelliteService.GetItemsNumber();

Console.WriteLine(itemsCount.ToString());