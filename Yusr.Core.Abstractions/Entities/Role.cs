using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Enums;

namespace Yusr.Core.Abstractions.Entities
{
    public class Role : BaseTenantEntity
    {
        public string Name { get; private set; } = string.Empty;
        public List<string>? ErpPermissions { get; private set; } = [];
        public List<string>? BusPermissions { get; private set; } = [];

        private static readonly Dictionary<AppModule, Func<Role, List<string>>> _getPermissions = new()
        {
            [AppModule.Erp] = r => r.ErpPermissions ?? [],
            [AppModule.Bus] = r => r.BusPermissions ?? [],
        };

        private static readonly Dictionary<AppModule, Action<Role, List<string>>> _setPermissions = new()
        {
            [AppModule.Erp] = (r, p) => r.ErpPermissions = p,
            [AppModule.Bus] = (r, p) => r.BusPermissions = p,
        };

        public List<string> Permissions
        {
            get => _getPermissions.TryGetValue(AppModuleConfig.Module, out var get) ? get(this) : [];

            private set
            {
                if (_setPermissions.TryGetValue(AppModuleConfig.Module, out var set))
                    set(this, value);
            }
        }

        public static Role Create(string name, List<string> permissions)
        {
            return new Role 
            { 
                Name = name, 
                Permissions = permissions 
            };
        }

        public Role Update(string name, List<string> permissions)
        {
            Name = name;
            Permissions = permissions;
            return this;
        }
    }
}
