using GetStoreAppInstaller.Extensions.DataType.Enums;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class DistinctQualifierInfo
    {
        public QualifierType QualifierType { get; set; }

        public uint OperandValueOffset { get; set; }
    }
}
