using System.Collections.Generic;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class QualifierSet
    {
        public ushort Index { get; set; }

        public IReadOnlyList<Qualifier> QualifiersList { get; set; }
    }
}
