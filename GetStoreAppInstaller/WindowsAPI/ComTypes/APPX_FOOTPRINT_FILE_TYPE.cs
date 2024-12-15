namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指定包中占用空间文件的类型。
    /// </summary>
    public enum APPX_FOOTPRINT_FILE_TYPE
    {
        /// <summary>
        /// 包清单。
        /// </summary>
        APPX_FOOTPRINT_FILE_TYPE_MANIFEST = 0,

        /// <summary>
        /// 包块映射。
        /// </summary>
        APPX_FOOTPRINT_FILE_TYPE_BLOCKMAP = 1,

        /// <summary>
        /// 包签名。
        /// </summary>
        APPX_FOOTPRINT_FILE_TYPE_SIGNATURE = 2,

        /// <summary>
        /// 用于代码完整性检查的代码签名目录文件。
        /// </summary>
        APPX_FOOTPRINT_FILE_TYPE_CODEINTEGRITY = 3,

        /// <summary>
        /// 用于流式安装的内容组映射。
        /// </summary>
        APPX_FOOTPRINT_FILE_TYPE_CONTENTGROUPMAP = 4
    }
}
