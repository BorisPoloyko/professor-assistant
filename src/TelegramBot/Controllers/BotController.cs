using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient bot;

        public BotController(ITelegramBotClient bot)
        {
            this.bot = bot;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update, CancellationToken cancellationToken)
        {
            var response = $"Hello, {update.Message?.From?.Username ?? "username"}. You have said: {update.Message?.Text ?? "nothing"}";
            await bot.SendTextMessageAsync(update.Message.From.Id, response);

            return Ok();
        }

        [HttpGet]
        public IActionResult Hello(CancellationToken cancellationToken)
        {
            return Ok("Hello");
        }
    }
}
