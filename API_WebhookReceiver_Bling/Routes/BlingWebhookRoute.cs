namespace API_WebhookReceiver_Bling.Routes
{
    public static class BlingWebhookRoute
    {
        public static void BlingWebhookRoutes(this WebApplication app)
        {
            app.MapGet("teste", () => "teste conexão");
        }
    }
}
