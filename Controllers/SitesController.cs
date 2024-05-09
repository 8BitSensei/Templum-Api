using JsonDb;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Templum.Api.Models;

namespace Templum.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SitesController : ControllerBase
    {
        private readonly ILogger<SitesController> _logger;
        private readonly IJsonDb _jsonDb;
        private readonly IZoteroClient _zoteroClient;
        private readonly IStrapiClient _strapiClient;
        private const string defaultCollection = "templum_sites";

        public SitesController(ILogger<SitesController> logger, IJsonDbFactory jsonDbFactory, IZoteroClient zoteroClient, IStrapiClient strapiClient)
        {
            _logger = logger;
            _jsonDb = jsonDbFactory.GetJsonDb();
            _zoteroClient = zoteroClient;
            _strapiClient = strapiClient;
        }

        [HttpGet]
        [Route("GetSites")]
        public async Task<IEnumerable<SiteModel>> Get(string collectionName = defaultCollection)
        {
            var sites = await _jsonDb.GetCollectionAsync<SiteMetadata>(collectionName);
            return sites.ElementAt(0).Sites;
        }

        [HttpGet]
        [Route("GetSite/{index}")]
        public async Task<IEnumerable<SiteModel>> GetByIndex(int index, string collectionName = defaultCollection)
        {
            var sites = await _jsonDb.GetCollectionAsync<SiteMetadata>(collectionName);
            return sites.ElementAt(0).Sites.Where(x => x.Index == index);
        }

        [HttpDelete]
        [Route("DeleteSite/{index}")]
        public async Task<StatusCodeResult> DeleteByIndex(int index, string collectionName = defaultCollection)
        {
            var sitesMeta = await _jsonDb.GetCollectionAsync<SiteMetadata>(collectionName);
            var sites = sitesMeta.ElementAt(0).Sites;
            var overwrite = sites.Where(x => x.Index == index).FirstOrDefault();
            if (overwrite != null)
            {
                sites.Remove(overwrite);
                await sitesMeta.WriteAsync();
                return StatusCode(200);
            }

            return StatusCode(404);
        }

        [HttpPost]
        [Route("PostSite")]
        public async Task<StatusCodeResult> Post(StrapiMetadata strapiSite, string collectionName = defaultCollection)
        {
            if (strapiSite.Event.Equals("entry.unpublish") || strapiSite.Event.Equals("entry.delete"))
                return await DeleteByIndex(strapiSite.Entry.TemplumId, collectionName);

            var sitesMeta = await _jsonDb.GetCollectionAsync<SiteMetadata>(collectionName);
            var newSite = new SiteModel
            {
                Index = strapiSite.Entry.TemplumId,
                Site = StripUnicode(strapiSite.Entry.Site),
                Description = StripUnicode(strapiSite.Entry.Description),
                Start = strapiSite.Entry.Start,
                End = strapiSite.Entry.End,
                Location = strapiSite.Entry.Location,
                Latitude = strapiSite.Entry.LocationExact.Coordinates.Lat.ToString(),
                Longitude = strapiSite.Entry.LocationExact.Coordinates.Long.ToString(),
                Status = strapiSite.Entry.Status
            };

            var tags = new StringBuilder();
            strapiSite.Entry.Tags.ForEach(x => tags.Append($"{x.Name},"));
            newSite.Tags = tags.ToString();

            var county = newSite.Location.Split(',')[1];
            var topCollection = await _zoteroClient.FindTopCollection(county.Trim());
            if (topCollection != null)
            {
                var subCollection = await _zoteroClient.FindSubCollection(topCollection, $"{newSite.Site.Trim()}, {newSite.Location}");
                if (subCollection != null)
                {
                    var bib = await _zoteroClient.GetBib(subCollection);
                    if (bib != null)
                        newSite.Bibliography = bib;
                }
            }

            var sites = sitesMeta.ElementAt(0).Sites;
            var overwrite = sites.Where(x => x.Index == newSite.Index).FirstOrDefault();
            if (overwrite != null)
            {
                var i = sites.IndexOf(overwrite);
                sites[i] = newSite;
            }
            else
            {
                sites.Add(newSite);
            }

            await sitesMeta.WriteAsync();

            return StatusCode(200);
        }

        [HttpPost]
        [Route("PullBranch")]
        public async Task<StatusCodeResult> PullBranch(string collectionName = defaultCollection)
        {
            var strapiCollectionName = collectionName.Replace("_", "-");
            var sitesMeta = await _jsonDb.GetCollectionAsync<SiteMetadata>(collectionName);
            var sites = sitesMeta.ElementAt(0).Sites;

            //TODO: Properties are null
            var strapiData = await _strapiClient.GetEntries(strapiCollectionName);
            if (strapiData == null)
                return StatusCode(500);

            var strapiSites = strapiData.Data;
            foreach (var site in sites)
            {
                var siteTags = site.Tags?.Split(",");
                if (siteTags == null) 
                    siteTags = [];

                var strapiTags = new List<StrapiTag>();
                foreach (var tag in siteTags)
                {
                    strapiTags.Add(new StrapiTag
                    {
                        Name = tag
                    });
                }

                var newStrapiEntry = new StrapiEntry
                {
                    TemplumId = site.Index,
                    Site = site.Site,
                    Description = site.Description,
                    End = site.End,
                    Start = site.Start,
                    Location = site.Location,
                    Status = site.Status?.ToLower() ?? "unknown",
                    Tags = strapiTags,
                    LocationExact = new StrapiLocation
                    {
                        Coordinates = new StrapiCoordinates
                        {
                            Lat = site.Latitude is null ? null : float.Parse(site.Latitude),
                            Long = site.Longitude is null ? null : float.Parse(site.Longitude)
                        }
                    }
                };

                var findExistingEntry = strapiSites.Where(x => x.Attributes.TemplumId == site.Index).FirstOrDefault();
                if (findExistingEntry is null)
                {
                    var success = await _strapiClient.AddEntry(new StrapiRequestObject { Data = newStrapiEntry  }, strapiCollectionName);
                    if (!success) 
                        return StatusCode(500);
                }
                else
                {
                    await _strapiClient.UpdateEntry(new StrapiRequestObject { Data = newStrapiEntry }, strapiCollectionName, findExistingEntry.Attributes.StrapiId);
                }
            }

            return StatusCode(200);
        }

        private static string StripUnicode(string input)
        {
            return Regex.Replace(input, @"[^\u0000-\u007F]+", string.Empty);
        }
    }
}
