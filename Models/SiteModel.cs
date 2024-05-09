using System.Text.Json.Serialization;

namespace Templum.Api.Models
{
    public class SiteModel
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("site")]
        public string Site { get; set; }

        [JsonPropertyName("start")]
        public string Start {  get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("latitude")]
        public string Latitude {  get; set; }

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("bibliography")]
        public List<string> Bibliography { get; set; }
    }
}
