namespace TelegramBot.Model.Configurations
{
    public class BotConfiguration
    {
        public static readonly string Configuration = "BotConfiguration";
        public string BotToken { get; set; } = null!;

        public string WebhookUrl { get; set; } = null!;
    }
}
