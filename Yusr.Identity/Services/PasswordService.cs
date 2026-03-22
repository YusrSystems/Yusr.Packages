using Microsoft.AspNetCore.Identity;
using Yusr.Core.Abstractions.Entities;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Identity.Services
{
    public class PasswordService(IPasswordHasher<User> passwordHasher) : IPasswordService
    {
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public string Hash(User user, string providedPassword)
        {
            return _passwordHasher.HashPassword(user, providedPassword);
        }

        public bool Verify(User user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
