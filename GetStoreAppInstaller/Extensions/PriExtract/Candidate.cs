﻿using GetStoreAppInstaller.Extensions.DataType.Enums;
using System;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class Candidate
    {
        public ushort QualifierSet { get; set; }

        public ResourceValueType Type { get; set; }

        public int? SourceFileIndex { get; set; }

        public (ushort DataItemSection, ushort DataItemIndex) DataItemSectionAndIndex { get; set; }

        public ByteSpan Data { get; set; }
    }
}
