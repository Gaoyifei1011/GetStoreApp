using System.Collections.Generic;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class ResourceMapScopeAndItem
    {
        public ushort Index { get; set; }

        public ResourceMapScopeAndItem Parent { get; set; }

        public string Name { get; set; }

        public IReadOnlyList<ResourceMapScopeAndItem> Children { get; internal set; }
    }
}
