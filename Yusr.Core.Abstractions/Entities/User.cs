namespace Yusr.Core.Abstractions.Entities
{
    public class User : BaseTenantEntity
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public long BranchId { get; private set; }
        public long RoleId { get; private set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;

        public static User Create(string username, bool isActive, long branchId, long roleId)
        {
            return new User
            {
                Username = username,
                IsActive = isActive,
                BranchId = branchId,
                RoleId = roleId
            };
        }

        public User Update(string username, bool isActive, long branchId, long roleId)
        {
            Username = username;
            IsActive = isActive;
            BranchId = branchId;
            RoleId = roleId;
            return this;
        }
        public User ChangePassword(string password)
        {
            Password = password;
            return this;
        }
    }
}
