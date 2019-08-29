using Newtonsoft.Json;

namespace Monaco.DataHandler.Models
{
    /// <summary>
    /// Represents food facility truck location details
    /// </summary>
    public class TruckInfo
    {
        [JsonProperty(PropertyName = "distanceInMeters")]

        public decimal DistanceInMeters { get; set; }

        [JsonProperty(PropertyName = "position")]
        public GeoCoordinate Position { get; set; }

        [JsonProperty(PropertyName = "facilityType")]
        public string FacilityType { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Applicant { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public int? Zip { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string LocationDescription { get; set; }

        [JsonProperty(PropertyName = "daysHours")]
        public string DaysHours { get; set; }
    }
}
