using System.Text.Json.Serialization;

namespace Templum.Api.Models
{
    public class StrapiMetadata
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("entry")]
        public StrapiEntry Entry { get; set; }
    }

    public class StrapiEntry 
    {
        [JsonPropertyName("id")]
        public int StrapiId { get; set; }
        [JsonPropertyName("index")]
        public int TemplumId { get; set; }
        [JsonPropertyName("site")]
        public string Site { get; set; }
        [JsonPropertyName("start")]
        public string? Start {  get; set; }
        [JsonPropertyName("end")]
        public string? End { get; set; }
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("locationExact")]
        public StrapiLocation LocationExact { get; set; }
        [JsonPropertyName("tags")]
        public List<StrapiTag> Tags { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("link")]
        public StrapiLink? Link { get; set; }
    }

    public class StrapiLink 
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class StrapiLocation 
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("geohash")]
        public string? Geohash { get; set; }
        [JsonPropertyName("coordinates")]
        public StrapiCoordinates Coordinates { get; set; }
    }

    public class StrapiCoordinates 
    {
        [JsonPropertyName("lat")]
        public float? Lat {  get; set; }
        [JsonPropertyName("lng")]
        public float? Long { get; set; }
    }

    public class StrapiTag 
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
