using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.Controllers.Base
{
    [Authorize]
    [ApiController]
    public abstract class AuthBaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current authenticated user from the request context.
        /// This user is automatically loaded by the UserFetchMiddleware.
        /// </summary>
        protected User? CurrentUser => HttpContext.Items["User"] as User;

        /// <summary>
        /// Gets the current user ID from the JWT claims.
        /// </summary>
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Gets the current user email from the JWT claims.
        /// </summary>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Validates that the current user is loaded and returns it.
        /// Returns UnauthorizedResult if user is not found.
        /// </summary>
        protected ActionResult<User> ValidateCurrentUser()
        {
            if (CurrentUser == null)
            {
                return Unauthorized("Unable to resolve current user");
            }
            return CurrentUser;
        }

        /// <summary>
        /// Gets the current user or returns an Unauthorized response if not found.
        /// Use this in actions where you need to ensure the user exists.
        /// </summary>
        protected User? GetCurrentUserOrThrow()
        {
            if (CurrentUser == null)
            {
                throw new UnauthorizedAccessException("Current user not found in request context");
            }
            return CurrentUser;
        }
        protected int CurrentUserId => GetCurrentUserId();

        protected int CurrentCompanyId => CurrentUser?.CompanyId ?? 0;
    }
}