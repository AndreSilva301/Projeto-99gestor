using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.Controllers.Base
{
    [ApiController]
    [Authorize]
    public abstract class AuthControllerBase : ControllerBase
    {
        /// <summary>
        /// Returns the current authenticated user based on claims.
        /// </summary>
        protected User? GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(ClaimTypes.Email);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return null;

            return new User
            {
                Id = userId,
                Email = emailClaim?.Value ?? string.Empty,
                IsActive = true,
            };
        }
    }
}
