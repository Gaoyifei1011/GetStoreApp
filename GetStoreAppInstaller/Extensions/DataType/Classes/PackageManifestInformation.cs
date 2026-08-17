using System.Collections.Generic;
using Windows.System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    internal class PackageManifestInformation
    {
        /// <summary>
        /// 应用信息
        /// </summary>
        internal Dictionary<ProcessorArchitecture, string> ApplicationDict { get; set; }

        /// <summary>
        /// 语言信息
        /// </summary>
        internal List<string> LanguageList { get; set; }

        /// <summary>
        /// 规模信息
        /// </summary>
        internal List<string> ScaleResourceList { get; set; }
    }
}
