using Monaco.DataHandler.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Monaco.DataHandler
{
    /// <summary>
    /// Represents the main food truck finder functionality
    /// </summary>
    public class TruckLocator
    {
        private const string _mapsAPIClosestPointURL = "https://atlas.microsoft.com/spatial/closestPoint/json";

        /// <summary>
        /// Locates 5 closest points to a given point. It uses Maps API closest point algorithm. After the points are identified, 
        /// more properties like business name, type, hours of operation are appended to the response 
        /// </summary>
        /// <param name="sqlConnection">static connection object</param>
        /// <param name="lat">source latitude coordinate</param>
        /// <param name="lon">source longitude coordinate</param>
        /// <param name="mapsDataUUID">UUID that was returned by Maps API when the data was uploaded</param>
        /// <param name="mapsSubsKey">Account key to access Maps API</param>
        /// <returns></returns>
        public async Task<TruckInfoResponse> Locate(SqlConnection sqlConnection, double lat, double lon, string mapsDataUUID, string mapsSubsKey)
        {
            Stopwatch sw = Stopwatch.StartNew();

            TruckInfoResponse resp = new TruckInfoResponse();

            // query MapsAPI Spatial - Get Closest Point
            var qString = $"?subscription-key={mapsSubsKey}&api-version=1.0&udid={mapsDataUUID}&lat={lat}&lon={lon}&numberOfClosestPoints=5";
            var client = new RestClient(_mapsAPIClosestPointURL + qString);
            var response = await client.ExecuteGetTaskAsync(new RestRequest());
            dynamic closestPoints = JsonConvert.DeserializeObject(response.Content);

            // inspect the response
            if(closestPoints != null)
            {
                // create response object from Maps API response

                resp.Trucks = new List<TruckInfo>();

                var points = closestPoints.result;
                // loop through closest points 
                foreach(var point in points)
                {
                    var t = await getTruckInfo(sqlConnection, point);
                    resp.Trucks.Add(t);
                }

                resp.Header = new ResponseSummary
                {
                    ReqestedLocation = new GeoCoordinate
                    {
                        Latitude = lat,
                        Longitude = lon
                    },
                    ElapsedMilliseconds = sw.ElapsedMilliseconds
                };

            }

            return resp;

        }

        /// <summary>
        /// Maps API data parser to create TruckInfo object
        /// </summary>
        /// <param name="point">data from Maps API</param>
        /// <returns>TuckInfo object</returns>
        private async Task<TruckInfo> getTruckInfo(SqlConnection sqlConnection, dynamic point)
        {
            // build the return object
            var truckInfo = new TruckInfo
            {
                DistanceInMeters = Convert.ToDecimal(point.distanceInMeters),
                Position = new GeoCoordinate
                {
                    Latitude = Convert.ToDouble(point.position.lat),
                    Longitude = Convert.ToDouble(point.position.lon)
                }
            };

            // get truck details from permit database, using geometryId (from Maps API) which is PK
            string sqlQuery = $"SELECT [FacilityType],[Applicant],[Address],[Zip_Codes],[LocationDescription],[dayshours] FROM [dbo].[FacilityPermit] WHERE [locationid] = {point.geometryId}";

            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            await sqlConnection.OpenAsync();
            using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
            {
                // call Read before accessing data.
                if (await sqlReader.ReadAsync())
                {
                    truckInfo.FacilityType = sqlReader[0] == null ? string.Empty : sqlReader[0].ToString();
                    truckInfo.Applicant = sqlReader[1] == null ? string.Empty : sqlReader[1].ToString();
                    truckInfo.Address = sqlReader[2] == null ? string.Empty : sqlReader[2].ToString();
                    truckInfo.Zip = sqlReader[3] == null ? 0 : Convert.ToInt32(sqlReader[3]);
                    truckInfo.LocationDescription = sqlReader[4] == null ? string.Empty : sqlReader[4].ToString();
                    truckInfo.DaysHours = sqlReader[5] == null ? string.Empty : sqlReader[5].ToString();
                }
            }
            sqlConnection.Close();
            
            // -- EntityFramework -- //
            // get truck details from permit database, using geometryId (from Maps API) which is PK
            //int locId = Convert.ToInt32(point.geometryId);
            //var permit = await _dbContext.FacilityPermits.Where(p => p.locationid == locId).FirstOrDefaultAsync();
            //if (permit != null)
            //{
            //    truckInfo.FacilityType = permit.FacilityType;
            //    truckInfo.Applicant = permit.Applicant;
            //    truckInfo.Address = permit.Address;
            //    truckInfo.Zip = permit.Zip_Codes;
            //    truckInfo.LocationDescription = permit.LocationDescription;
            //    truckInfo.DaysHours = permit.dayshours;
            //}

            return truckInfo;
        }
    }
}
