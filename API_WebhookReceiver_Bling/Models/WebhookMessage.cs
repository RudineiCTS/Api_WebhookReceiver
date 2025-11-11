using System.Text.Json;
namespace API_WebhookReceiver_Bling.Models
{
    public class WebhookMessage
    {
        public string EventId { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }
        public string Event { get; set; }
        public string CompanyId { get; set; }
        public JsonElement Data { get; set; }

    }
}
