using System.Text.Json.Serialization;

namespace Templum.Api.Models
{
    public class StrapiRequestObject 
    {
        [JsonPropertyName("data")]
        public StrapiEntry Data { get; set; }
    }

    public class StrapiApiObject 
    {
        [JsonPropertyName("data")]
        public StrapiData Data { get; set; }
    }

    public class StrapiApiArrayObject
    {
        [JsonPropertyName("data")]
        public List<StrapiData> Data { get; set; }
    }

    public class StrapiData 
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("attributes")]
        public StrapiEntry Attributes { get; set; }
    }

}
