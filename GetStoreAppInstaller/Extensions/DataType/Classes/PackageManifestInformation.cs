using System.Collections.Generic;
using Windows.System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    public class PackageManifestInformation
    {
        /// <summary>
        /// 应用信息
        /// </summary>
        public Dictionary<ProcessorArchitecture, string> ApplicationDict { get; set; }

        /// <summary>
        /// 语言信息
        /// </summary>
        public Dictionary<string, string> LanguageResourceDict { get; set; }

        /// <summary>
        /// 规模信息
        /// </summary>
        public List<string> ScaleResourceList { get; set; }
    }
}
