namespace Yusr.Core.Abstractions.Entities
{
    public interface IUser
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool IsActive { get; set; }
        long RoleId { get; set; }

        ITenant Tenant { get; set; }
        IRole Role { get; set; }
    }
}
