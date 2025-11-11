using API_WebhookReceiver_Bling.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using static API_WebhookReceiver_Bling.utils.FilaInterna;

namespace API_WebhookReceiver_Bling.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookRecieverController : ControllerBase
    {
        private const string CLIENT_SECRET = "SEU_CLIENT_SECRET_AQUI"; // coloque o segredo do seu app no Bling aqui

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook()
        {
            // 1. Ler o RAW body
            Request.EnableBuffering(); // permite ler o body mais de uma vez
            string rawBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0; // reseta para permitir desserialização posteriormente
            }

            // 2. Obter o header enviado pelo Bling
            if (!Request.Headers.TryGetValue("X-Bling-Signature-256", out var signatureHeader))
                return Unauthorized("Cabeçalho de assinatura ausente.");

            var signatureReceived = signatureHeader.ToString().Replace("sha256=", "");

            // 3. Calcular o hash local
            var signatureGenerated = GenerateHmacSha256(rawBody, CLIENT_SECRET);

            // 4. Comparar (verifica se a requisição é confiável)
            if (!signatureGenerated.Equals(signatureReceived, StringComparison.OrdinalIgnoreCase))
                return Unauthorized("Assinatura inválida.");

            // 5. Agora sim podemos desserializar para o modelo
            var payload = JsonSerializer.Deserialize<WebhookMessage>(rawBody);

            // 6. Processa a mensagem (se quiser enfileirar, logar, etc.)
            Console.WriteLine($"Webhook recebido: Evento={payload.Event} Empresa={payload.CompanyId}");


            EventQueue.Queue.Writer.TryWrite(payload);

            return Ok();
        }

        private static string GenerateHmacSha256(string payload, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(payloadBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
