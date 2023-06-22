using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services.Interfaces.Processing;

namespace TelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BotController : ControllerBase
    {
        private readonly IUpdateProcessor _updateProcessor;
        public BotController(IUpdateProcessor updateProcessor)
        {
            _updateProcessor = updateProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update, CancellationToken cancellationToken)
        {
            ValidateUpdateRequest(update);

            await _updateProcessor.ProcessUpdate(update, cancellationToken);

            return Ok();
        }

        

        /// <summary>
        /// Testing endpoint that indicates that the application is running
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Hello(CancellationToken cancellationToken)
        {
            return Ok("Hello");
        }

        private void ValidateUpdateRequest(Update update)
        {
            if (update.Type != UpdateType.Message || update.Message == null)
            {
                throw new ArgumentOutOfRangeException(nameof(update), "Only not empty messages are allowed.");
            }

            if (update.Message?.From?.Id == null)
            {
                throw new ArgumentOutOfRangeException(nameof(update), "Cannot determine the sender.");
            }
        }
    }
}
