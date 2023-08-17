using System;

namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用包签名类型
    /// </summary>
    [Flags]
    public enum PackageSignType
    {
        /// <summary>
        /// 该包未签名。 例如，从布局运行的 Visual Studio 项目 (F5) 。
        /// </summary>
        None = 1,

        /// <summary>
        /// 包使用不分类为 Enterprise、 Store或 System的受信任证书进行签名。 例如，由 ISV 签名以在 Microsoft Store 外部析构的应用程序。
        /// </summary>
        Developer = 2,

        /// <summary>
        /// 包使用根颁发机构颁发的证书进行签名，该证书的验证要求高于一般公共机构。
        /// </summary>
        Enterprise = 4,

        /// <summary>
        /// 包由 Windows 应用商店签名。
        /// </summary>
        Store = 8,

        /// <summary>
        /// 包由也用于对 Windows 操作系统进行签名的证书进行签名。 这些包可能具有未授予普通应用的其他功能。 例如，内置的“设置”应用。
        /// </summary>
        System = 16
    }
}
