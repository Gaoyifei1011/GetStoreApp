using System;

namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 标识程序包面向的设备系列的数据模型
    /// </summary>
    internal class TargetDeviceFamilyModel
    {
        /// <summary>
        /// 应用面向的目标设备系列的名称。
        /// </summary>
        internal string TargetDeviceName { get; set; }

        /// <summary>
        /// 应用面向的设备系列的最低版本。
        /// </summary>
        internal Version MinVersion { get; set; }

        /// <summary>
        /// 应用所针对的设备系列的最高版本
        /// </summary>
        internal Version MaxVersionTested { get; set; }
    }
}
