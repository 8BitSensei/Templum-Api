namespace Templum.Api
{
    public interface IReceiveWebhook
    {
        Task<string> ProcessRequest(string requestBody);
    }
}
