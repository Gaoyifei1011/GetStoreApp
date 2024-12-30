using System;
using System.Collections.Generic;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class CandidateSet
    {
        public Tuple<int, int> ResourceMapSectionAndIndex { get; set; }

        public ushort DecisionIndex { get; set; }

        public IReadOnlyList<Candidate> CandidatesList { get; set; }
    }
}
