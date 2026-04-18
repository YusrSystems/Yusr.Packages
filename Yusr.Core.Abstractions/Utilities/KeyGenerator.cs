using System.Security.Cryptography;

namespace Yusr.Core.Abstractions.Utilities
{
    public static class KeyGenerator
    {
        public static string GenerateNanoid(int length = 10)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);

            return Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                [..length];
        }
    }
}

