using AngleSharp;
using AngleSharp.Html.Parser;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Templum.Api
{
    public class ZoteroClient : IZoteroClient
    {
        private readonly HttpClient _httpClient;
        private const string groupKey = "4536134";
        public ZoteroClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Zotero");
        }

        public async Task<string?> FindTopCollection(string searchName)
        {
            List<ZoteroCollection>? topCollections;
            using HttpResponseMessage response = await _httpClient.GetAsync($"groups/{groupKey}/collections/top?limit=100");
            if (!response.IsSuccessStatusCode) 
                return null;

            topCollections = await response?.Content?.ReadFromJsonAsync<List<ZoteroCollection>?>();
            if (topCollections == null)
                return null;

            var queryCollection = topCollections.Where(x => x.Data.Name.Equals(searchName.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return queryCollection?.Data?.Key;
        }

        public async Task<string?> FindSubCollection(string parentCollection, string searchName)
        {
            List<ZoteroCollection>? topCollections;
            using HttpResponseMessage response = await _httpClient.GetAsync($"groups/{groupKey}/collections/{parentCollection}/collections?limit=100");
            if (!response.IsSuccessStatusCode)
                return null;

            topCollections = await response?.Content?.ReadFromJsonAsync<List<ZoteroCollection>?>();
            if (topCollections == null)
                return null;

            var queryCollection = topCollections.Where(x => x.Data.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return queryCollection?.Data?.Key;
        }

        public async Task<List<string>?> GetBib(string collection)
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"groups/{groupKey}/collections/{collection}/items?format=bib");
            if (!response.IsSuccessStatusCode)
                return [];

            var bibHtml = await response?.Content?.ReadAsStringAsync();
            if (bibHtml == null)
                return [];

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(bibHtml);
            var selector = document.All.Where(x => x.LocalName.Equals("div") &&
                                                   x.HasAttribute("class") &&
                                                   x.GetAttribute("class").Equals("csl-entry"));
            
            List<string> array = new List<string>();
            foreach (var item in selector) 
            {
                var stringItem = item.InnerHtml;
                if(stringItem is not null)
                    array.Add(StripUnicode(StripHTML(stringItem)));
            }

            return array;
        }

        private static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        private static string StripUnicode(string input)
        {
            return Regex.Replace(input, @"[^\u0000-\u007F]+", string.Empty);
        }
    }

    public class ZoteroCollection 
    {
        [JsonPropertyName("data")]
        public ZoteroData Data { get; set; }
    }

    public class ZoteroData 
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
