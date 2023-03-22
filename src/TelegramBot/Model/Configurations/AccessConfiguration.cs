namespace TelegramBot.Model.Configurations
{
    public class AccessConfiguration
    {
        public static readonly string Configuration = "Access";
        public IEnumerable<string> AdminIds { get; set; }
    }
}
