using System.Text.Json.Serialization;

namespace Templum.Api.Models
{
    public class SiteMetadata
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sites")]
        public List<SiteModel> Sites { get; set; }
        [JsonPropertyName("dates")]
        public Dictionary<string, string> Dates { get; set; }
    }
}
