using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monaco.DataHandler.Models
{
    /// <summary>
    /// Represents API response object
    /// </summary>
    public class TruckInfoResponse
    {
        [JsonProperty(PropertyName = "summary")]
        public ResponseSummary Header { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<TruckInfo> Trucks { get; set; }
    }
}
