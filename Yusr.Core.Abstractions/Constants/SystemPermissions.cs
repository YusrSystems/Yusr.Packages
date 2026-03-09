using System.Collections.Frozen;

namespace Yusr.Core.Abstractions.Constants
{
    public abstract class BaseSystemPermissions(Dictionary<string, string[]> registry)
    {
        private readonly FrozenDictionary<string, string[]> _resourceMap = registry.ToFrozenDictionary();
        private readonly FrozenSet<string> _all = registry
            .SelectMany(rm => rm.Value.Select(action => Create(rm.Key, action)))
            .ToFrozenSet();

        public FrozenDictionary<string, string[]> ResourceMap => _resourceMap;
        public FrozenSet<string> All => _all;
        public List<string> GetResources() => [.. _resourceMap.Keys];
        public static string Create(string resource, string action) => $"{resource}:{action}";
    }
}