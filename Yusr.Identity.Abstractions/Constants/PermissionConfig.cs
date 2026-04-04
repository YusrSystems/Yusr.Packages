using Yusr.Identity.Abstractions.Interfaces;

namespace Yusr.Identity.Abstractions.Constants
{
    public static class PermissionConfig
    {
        public static Func<string, string, string> Formatter { get; set; } = ISystemPermissions.Create;
        public static void SetFormat(Func<string, string, string> formatFunc)
        {
            Formatter = formatFunc;
        }
    }
}
