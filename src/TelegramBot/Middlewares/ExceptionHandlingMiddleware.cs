using Microsoft.AspNetCore.Mvc.Controllers;
using Telegram.Bot;
using TelegramBot.Model.Exceptions;

namespace TelegramBot.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ITelegramBotClient _bot;

        public ExceptionHandlingMiddleware(RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger,
            ITelegramBotClient bot)
        {
            _next = next;
            _logger = logger;
            _bot = bot;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnknownBotCommandException ex)
            {
                _logger.LogError(ex, "Unknown bot command was sent");
                try
                {
                    if (context.Items.TryGetValue("userId", out var userId) && userId is long id)
                    {
                        await _bot.SendTextMessageAsync(id, "Unknown command");
                    }
                }
                catch (Exception botException)
                {
                    _logger.LogError(botException, "Exception occurred");
                }
                context.Response.StatusCode = 200;
            }
            catch (DetachedDialogException ex)
            {
                _logger.LogError(ex, "Detached command");
                try
                {
                    if (context.Items.TryGetValue("userId", out var userId) && userId is long id)
                    {
                        await _bot.SendTextMessageAsync(id, "Cannot find a command that was corresponding to a message. Please enter a new command");
                    }
                }
                catch (Exception botException)
                {
                    _logger.LogError(botException, "Exception occurred");
                }
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred");
                context.Response.StatusCode = 200;
            }
        }
    }
}
