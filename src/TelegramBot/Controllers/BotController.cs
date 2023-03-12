using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BotController : ControllerBase
    {
        [HttpPost]
        public IActionResult Update([FromBody] Update update, CancellationToken cancellationToken)
        {
            var response = $"Hello, {update.Message?.From?.Username ?? "username"}. You have said: {update.Message?.Text ?? "nothing"}";
            return Ok(response);
        }

        [HttpGet]
        public IActionResult Hello(CancellationToken cancellationToken)
        {
            return Ok("Hello");
        }
    }
}
