using Templum.Api.Models;

namespace Templum.Api
{
    public interface IStrapiClient
    {
        public Task<StrapiApiArrayObject?> GetEntries(string type);

        public Task<bool> AddEntry(StrapiRequestObject data, string type);

        public Task<bool> UpdateEntry(StrapiRequestObject data, string type, int strapiId);
    }
}
