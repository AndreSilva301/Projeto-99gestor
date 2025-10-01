using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Infrastructure.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<User> _hasher = new();

        public static string Hash(string plainPassword, User user)
        {
            return _hasher.HashPassword(user, plainPassword);
        }

        public static bool Verify(string hashedPassword, string plainPassword, User user)
        {
            var result = _hasher.VerifyHashedPassword(user, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
