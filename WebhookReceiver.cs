namespace Templum.Api
{
    public class WebhookReceiver : IReceiveWebhook
    {
        public Task<string> ProcessRequest(string requestBody)
        {
            Console.WriteLine($"Request Body: {requestBody}");

            return Task.FromResult("{\"message\" : \"Thanks! We got your webhook\"}");
        }
    }
}
