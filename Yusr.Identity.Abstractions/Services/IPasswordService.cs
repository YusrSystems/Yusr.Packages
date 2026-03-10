using Yusr.Core.Abstractions.Entities;

namespace Yusr.Identity.Abstractions.Services
{
    public interface IPasswordService
    {
        string Hash(IUser user, string providedPassword);
        bool Verify(IUser user, string hashedPassword, string providedPassword);
    }
}
