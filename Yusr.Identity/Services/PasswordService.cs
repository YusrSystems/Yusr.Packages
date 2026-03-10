using Microsoft.AspNetCore.Identity;
using Yusr.Core.Abstractions.Entities;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Bus.Identity.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<IUser> _passwordHasher;

        public PasswordService(IPasswordHasher<IUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string Hash(IUser user, string providedPassword)
        {
            return _passwordHasher.HashPassword(user, providedPassword);
        }

        public bool Verify(IUser user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
