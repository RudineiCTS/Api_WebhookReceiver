
using System.Text.Json;
using static API_WebhookReceiver_Bling.utils.FilaInterna;

namespace API_WebhookReceiver_Bling.Service
{
    public class WebhookProcessorService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly string _jsonPath = Path.Combine(AppContext.BaseDirectory, "eventos.json"); //para teste

        public WebhookProcessorService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("WebhookProcessorService iniciado ✅");

            try
            {
                await foreach (var evt in EventQueue.Queue.Reader.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine("Evento recebido na fila ✅");
                    await SalvarEventoLocalAsync(evt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no WebhookProcessorService: {ex.Message}");
            }

            Console.WriteLine("WebhookProcessorService finalizado 🛑");
        }


        private async Task SalvarEventoLocalAsync(object evt)
        {
            // Caminho fixo: salva na raiz do projeto
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(folder); // garante que a pasta exista

            var filePath = Path.Combine(folder, "eventos.json");

            var json = JsonSerializer.Serialize(evt, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.AppendAllTextAsync(filePath, json + Environment.NewLine + "-----" + Environment.NewLine);

            Console.WriteLine($"Evento salvo em {filePath}");
        }

    }
}
