namespace Templum.Api
{
    public interface IZoteroClient
    {
        public Task<string?> FindTopCollection(string searchName);

        public Task<string?> FindSubCollection(string parentCollection, string searchName);

        public Task<List<string>?> GetBib(string collection);
    }
}
