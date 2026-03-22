using Yusr.Core.Abstractions.Entities;

namespace Yusr.Identity.Abstractions.Services
{
    public interface IPasswordService
    {
        string Hash(User user, string providedPassword);
        bool Verify(User user, string hashedPassword, string providedPassword);
    }
}
