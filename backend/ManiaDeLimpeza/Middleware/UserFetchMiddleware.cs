using ManiaDeLimpeza.Application.Interfaces;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.Middleware
{
    public class UserFetchMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserFetchMiddleware> _logger;

        public UserFetchMiddleware(RequestDelegate next, ILogger<UserFetchMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            try
            {
                // Verifica se o usuário está autenticado
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    
                    if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                    {
                        var user = await userService.GetByIdAsync(userId);
                        if (user != null)
                        {
                            // Adiciona o usuário no contexto HTTP para acesso posterior
                            context.Items["User"] = user;
                            _logger.LogDebug("User {UserId} loaded and added to request context", userId);
                        }
                        else
                        {
                            _logger.LogWarning("User {UserId} not found in database but has valid token", userId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Unable to parse user ID from token claims");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user from database");
                // Continue with the request even if user fetch fails
                // The controller can handle the case where User is null
            }

            await _next(context);
        }
    }
}