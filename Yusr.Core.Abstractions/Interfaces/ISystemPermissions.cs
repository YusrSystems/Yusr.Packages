using System.Collections.Frozen;

namespace Yusr.Core.Abstractions.Interfaces
{
    public interface ISystemPermissions
    {
        static abstract FrozenDictionary<string, string[]> ResourceMap { get; }
        static abstract FrozenSet<string> All { get; }
        public static List<string> GetResources<T>() where T : ISystemPermissions => [.. T.ResourceMap.Keys];
        public static string Create(string resource, string action) => $"{resource}:{action}";
    }
}