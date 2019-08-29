using Newtonsoft.Json;

namespace Monaco.DataHandler.Models
{
    /// <summary>
    /// represents API response summary
    /// </summary>
    public class ResponseSummary
    {
        [JsonProperty(PropertyName = "requestedLocation")]
        public GeoCoordinate ReqestedLocation { get; set; }

        [JsonProperty(PropertyName = "responseTime")]
        public long ElapsedMilliseconds { get; set; }
        
    }
}
