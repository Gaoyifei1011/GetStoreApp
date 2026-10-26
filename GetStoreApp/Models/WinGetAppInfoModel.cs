using GetStoreApp.Extensions.DataType.Enums;
using System;

namespace GetStoreApp.Models
{
    /// <summary>
    /// WinGet 应用信息数据模型
    /// </summary>
    internal class WinGetAppInfoModel
    {
        /// <summary>
        /// 信息数据类别
        /// </summary>
        internal WinGetAppInfoKind WinGetAppInfoKind { get; set; }

        /// <summary>
        /// 信息数据类别名称
        /// </summary>
        internal string ClassName { get; set; }

        /// <summary>
        /// 信息数据字符串内容
        /// </summary>
        internal string StringContent { get; set; }

        /// <summary>
        /// 信息数据链接内容
        /// </summary>
        internal Uri UriContent { get; set; }
    }
}
