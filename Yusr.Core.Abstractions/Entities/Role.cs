namespace Yusr.Core.Abstractions.Entities
{
    public class Role : BaseTenantEntity
    {
        public string Name { get; private set; } = string.Empty;
        public List<string> Permissions { get; private set; } = new();

        public static Role Create(string name, List<string> permissions)
        {
            return new Role
            {
                Name = name,
                Permissions = permissions ?? new List<string>()
            };
        }

        public void Update(string name, List<string> permissions)
        {
            Name = name;
            Permissions = permissions ?? new List<string>();
        }
    }
}
