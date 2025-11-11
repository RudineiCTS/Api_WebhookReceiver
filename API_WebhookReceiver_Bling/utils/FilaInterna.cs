using API_WebhookReceiver_Bling.Models;
using System.Threading.Channels;

namespace API_WebhookReceiver_Bling.utils
{
    public class FilaInterna
    {
        public static class EventQueue
        {
            public static readonly Channel<WebhookMessage> Queue = Channel.CreateUnbounded<WebhookMessage>();
        }
    }
}
