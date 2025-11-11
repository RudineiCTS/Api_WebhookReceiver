
using static API_WebhookReceiver_Bling.utils.FilaInterna;

namespace API_WebhookReceiver_Bling.Service
{
    public class WebhookProcessorService : BackgroundService
    {
        private readonly HttpClient _httpClient;

        public WebhookProcessorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var evt in EventQueue.Queue.Reader.ReadAllAsync(stoppingToken))
            {
                await _httpClient.PostAsJsonAsync("http://sua-outra-api/eventos", evt, stoppingToken);
            }
        }
    }
}
