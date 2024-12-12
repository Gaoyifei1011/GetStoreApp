namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指定包请求的功能或特权。
    /// </summary>
    public enum APPX_CAPABILITIES
    {
        /// <summary>
        /// 与 Internet 的传出连接的 Internet 连接。
        /// </summary>
        APPX_CAPABILITY_INTERNET_CLIENT = 1,

        /// <summary>
        /// Internet 连接，包括来自 Internet 的传入连接 - 应用可以通过防火墙向计算机发送或从计算机发送信息。 如果声明了此功能，则无需声明 APPX_CAPABILITY_INTERNET_CLIENT 。
        /// </summary>
        APPX_CAPABILITY_INTERNET_CLIENT_SERVER = 2,

        /// <summary>
        /// 家庭或工作网络 - 应用可以向或从你的计算机和同一网络上的其他计算机发送信息。
        /// </summary>
        APPX_CAPABILITY_PRIVATE_NETWORK_CLIENT_SERVER = 4,

        /// <summary>
        /// 文档库，包括添加、更改或删除文件的功能。 包只能访问它在清单中声明的文件类型。 该应用程序不能访问家庭组计算机上的文档库。
        /// </summary>
        APPX_CAPABILITY_DOCUMENTS_LIBRARY = 8,

        /// <summary>
        /// 图片库，包括添加、更改或删除文件的功能。 此功能还包括家庭组计算机上的图片库，以及本地连接的媒体服务器上的图片文件类型。
        /// </summary>
        APPX_CAPABILITY_PICTURES_LIBRARY = 16,

        /// <summary>
        /// 视频库，包括添加、更改或删除文件的功能。 此功能还包括家庭组计算机上的视频库，以及本地连接的媒体服务器上的视频文件类型。
        /// </summary>
        APPX_CAPABILITY_VIDEOS_LIBRARY = 32,

        /// <summary>
        /// 音乐库和播放列表，包括添加、更改或删除文件的功能。 此功能还包括家庭组计算机上的音乐库中的音乐库和播放列表，以及本地连接的媒体服务器上的音乐文件类型。
        /// </summary>
        APPX_CAPABILITY_MUSIC_LIBRARY = 64,

        /// <summary>
        /// 用于访问公司 Intranet 的 Windows 凭据。 该应用程序可以模拟网络上的用户。
        /// </summary>
        APPX_CAPABILITY_ENTERPRISE_AUTHENTICATION = 128,

        /// <summary>
        /// 软件和硬件证书或智能卡 - 用于在应用中识别你的身份。 你的雇主、银行或政府服务可能会使用此功能来识别你的身份。
        /// </summary>
        APPX_CAPABILITY_SHARED_USER_CERTIFICATES = 256,

        /// <summary>
        /// 可移动存储，如外部硬盘或 U 盘或 MTP 便携式设备，包括添加、更改或删除特定文件的功能。 此包只能访问它在清单中声明的文件类型。
        /// </summary>
        APPX_CAPABILITY_REMOVABLE_STORAGE = 512,

        APPX_CAPABILITY_APPOINTMENTS = 1024,
        APPX_CAPABILITY_CONTACTS = 2048,
    }
}
