using System.Text.Json;
using Templum.Api.Models;

namespace Templum.Api
{
    public class StrapiClient : IStrapiClient
    {
        private readonly HttpClient _httpClient;
        public StrapiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Strapi");
        }

        public async Task<bool> AddEntry(StrapiRequestObject data, string type)
        {
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/{type}", data);
            if (!response.IsSuccessStatusCode) 
            {
                var json = JsonSerializer.Serialize(data);
                return false;
            }

            return true;
        }

        public async Task<StrapiApiArrayObject?> GetEntries(string type)
        {
            StrapiApiArrayObject entries;
            using HttpResponseMessage response = await _httpClient.GetAsync($"api/{type}");
            if (!response.IsSuccessStatusCode)
                return null;

            entries = await response?.Content?.ReadFromJsonAsync<StrapiApiArrayObject>();
            if (entries == null)
                return null;

            return entries;
        }

        public async Task<bool> UpdateEntry(StrapiRequestObject data, string type, int strapiId)
        {
            using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/{type}/{strapiId}", data);
            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }
    }
}
