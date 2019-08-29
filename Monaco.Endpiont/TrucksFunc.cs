using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Monaco.DataHandler;
using Monaco.DataHandler.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Monaco.Endpiont
{
    public static class TrucksFunc
    {
        // static declaration to help with connection managemnt
        private static SqlConnection _sqlConnection = new SqlConnection();

        [FunctionName("Trucks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            #region validate input

            // validate input parameter

            string latStr = req.Query["lat"];
            string lonStr = req.Query["lon"];
            double lat, lon;

            if (string.IsNullOrWhiteSpace(latStr) ||
                string.IsNullOrWhiteSpace(lonStr) ||
                !Double.TryParse(latStr, out lat) ||
                !Double.TryParse(lonStr, out lon) ||
                !(lat >= -90 && lat <= 90) ||
                !(lon >= -180 && lon <= 180))
            {
                return new BadRequestObjectResult("Please pass valid values for parameters ?lat=<value>&lon=<value> on the query string");
            }

            #endregion

            log.LogInformation($"Trucks location req received for {lat},{lon}");

            #region fetch settings

            // connection string to SQL DB
            _sqlConnection.ConnectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");

            // get the UUID for Maps API data from conguration
            // TODO: Ideally should come from database after Maps API upload
            var mapsDataUUID = Environment.GetEnvironmentVariable("MapsData.UUID");

            // Maps API subscription key from Azure portal
            var mapsSubsKey = Environment.GetEnvironmentVariable("MapsData.SubsKey");

            #endregion

            TruckLocator locator = new TruckLocator();
            TruckInfoResponse resp = await locator.Locate(_sqlConnection, lat, lon, mapsDataUUID, mapsSubsKey);

            return (ActionResult)new OkObjectResult(resp);

            //catch(Exception ex)
            //{
            //    return (ActionResult)new BadRequestObjectResult(ex.ToString());
            //}
        }
    }
}
