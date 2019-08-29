using Newtonsoft.Json;

namespace Monaco.DataHandler.Models
{
    /// <summary>
    /// Represents a geographical location using latitude and longitude
    /// </summary>
    public class GeoCoordinate
    {
        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Longitude { get; set; }
    }
}
