namespace Yusr.Application.Abstractions.DTOs
{
    public class UserDto : BaseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public long BranchId { get; set; }
        public long RoleId { get; set; }
        public RoleDto? Role { get; set; } = new RoleDto();
        public BranchDto? Branch { get; set; } = new BranchDto();
    }
}
