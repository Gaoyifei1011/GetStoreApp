using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Version
{
    /// <summary>
    /// 包含文件的版本信息。 此信息独立于语言和代码页。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VS_FIXEDFILEINFO
    {
        /// <summary>
        /// 包含值0xFEEF04BD。 在搜索VS_FIXEDFILEINFO结构时，这与VS_VERSIONINFO结构的 szKey 成员一起使用。
        /// </summary>
        public uint dwSignature;

        /// <summary>
        /// 此结构的二进制版本号。 此成员的高序字包含主版本号，低序字包含次要版本号
        /// </summary>
        public uint dwStrucVersion;

        /// <summary>
        /// 文件二进制版本号中最重要的 32 位。 此成员与 dwFileVersionLS 一起使用，形成用于数字比较的 64 位值。
        /// </summary>
        public uint dwFileVersionMS;

        /// <summary>
        /// 文件的二进制版本号中最小有效 32 位。 此成员与 dwFileVersionMS 一起使用，形成用于数字比较的 64 位值。
        /// </summary>
        public uint dwFileVersionLS;

        /// <summary>
        /// 分发此文件的产品的二进制版本号中最重要的 32 位。 此成员与 dwProductVersionLS 一起使用，形成用于数字比较的 64 位值。
        /// </summary>
        public uint dwProductVersionMS;

        /// <summary>
        /// 分发此文件的产品的二进制版本号中最小有效 32 位。 此成员与 dwProductVersionMS 一起使用，形成用于数字比较的 64 位值。
        /// </summary>
        public uint dwProductVersionLS;

        /// <summary>
        /// 包含指定 dwFileFlags 中的有效位的位掩码。 仅当创建文件时定义位才有效。
        /// </summary>
        public uint dwFileFlagsMask;

        /// <summary>
        /// 包含指定文件的布尔属性的位掩码。 此成员可以包含以下一个或多个值。
        /// 应用程序可以合并这些值，以指示该文件是为另一个上运行的操作系统设计的。
        /// </summary>
        public uint dwFileFlags;

        /// <summary>
        /// 为此文件设计的操作系统。
        /// </summary>
        public uint dwFileOS;

        /// <summary>
        /// 文件的常规类型。
        /// </summary>
        public uint dwFileType;

        /// <summary>
        /// 文件的函数。 可能的值取决于 dwFileType 的值。 对于以下列表中未描述的 dwFileType 的所有值， dwFileSubtype 为零。
        /// </summary>
        public uint dwFileSubtype;

        /// <summary>
        /// 文件的 64 位二进制创建日期和时间中最重要的 32 位。
        /// </summary>
        public uint dwFileDateMS;

        /// <summary>
        /// 文件的 64 位二进制创建日期和时间的最小有效 32 位。
        /// </summary>
        public uint dwFileDateLS;
    }
}
