using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 为了配置而提交的结构任务栏图标。提供各种成员可以部分配置，根据 <see cref="IconDataMembers"/> 中的定义。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NOTIFYICONDATA
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// 接收与任务栏状态区域中的图标关联的通知消息的窗口句柄。命令行管理程序使用 hWnd 和 uID 来标识调用Shell_NotifyIcon时要操作的图标。
        /// </summary>
        public IntPtr WindowHandle;

        /// <summary>
        /// 任务栏图标的应用程序定义的标识符。命令行管理程序使用 hWnd 和 uID 来标识调用Shell_NotifyIcon时要操作的图标。
        /// 您可以通过为每个图标分配不同的 uID 来将多个图标与单个 hWnd 相关联。但是，此功能目前未使用。
        /// </summary>
        public uint TaskbarIconId;

        /// <summary>
        /// 指示哪些其他成员包含有效数据的标志。此成员可以是NIF_XXX常量的组合。
        /// </summary>
        public IconDataMembers ValidMembers;

        /// <summary>
        /// 应用程序定义的消息标识符。系统使用此标识符将通知发送到 hWnd 中标识的窗口。
        /// </summary>
        public uint CallbackMessageId;

        /// <summary>
        /// 应显示的图标的句柄。
        /// </summary>
        public IntPtr IconHandle;

        /// <summary>
        /// 包含标准工具提示文本的字符串。它最多可以包含 64 个字符，包括终止 NULL。对于版本 5.0 及更高版本，szTip 最多可以包含 128 个字符，包括终止 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ToolTipText;

        /// <summary>
        /// 图标的状态。请记住还要设置 <see cref="StateMask"/>。
        /// </summary>
        public IconState IconState;

        /// <summary>
        /// 一个值，指定检索或修改状态成员的哪些位。例如，将此成员设置为 <see cref="TaskbarNotification.Interop.IconState.Hidden"/> 只会检索项目的隐藏状态。
        /// </summary>
        public IconState StateMask;

        /// <summary>
        /// 包含气球工具提示文本的字符串。它最多可以包含 255 个字符。若要删除工具提示，请在 uFlags 中设置 NIF_INFO 标志，并将 szInfo 设置为空字符串。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string BalloonText;

        /// <summary>
        /// 主要用于在调用 <see cref="Shell32Library.Shell_NotifyIcon"/> 时设置版本<see cref="NotifyCommand.SetVersion"/>。
        /// 但是，对于旧版操作，同一成员也用于设置气球工具提示的超时。
        /// </summary>
        public uint VersionOrTimeout;

        /// <summary>
        /// 包含气球工具提示标题的字符串。此标题以粗体显示在文本上方。它最多可以包含 63 个字符。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BalloonTitle;

        /// <summary>
        /// 将图标添加到放置在标题左侧的气球工具提示中。如果 <see cref="BalloonTitle"/> 成员的长度为零，则不会显示该图标。
        /// </summary>
        public BalloonFlags BalloonFlags;

        /// <summary>
        /// Windows XP（Shell32.dll版本 6.0）及更高版本。
        /// Windows 7 及更高版本：标识图标的已注册 GUID。此值将覆盖 uID，并且是标识图标的推荐方法。
        /// Windows XP 到 Windows Vista：保留。
        /// </summary>
        public Guid TaskbarIconGuid;

        /// <summary>
        /// Windows Vista（Shell32.dll版本 6.0.6）及更高版本。应用程序提供的自定义气球图标的句柄，应独立于托盘图标使用。
        /// 如果此成员为非 NULL，并且设置了<see cref="TaskbarNotification.Interop.BalloonFlags.User"/>的flag，则此图标将用作气球图标。
        /// 如果此成员为 NULL，则执行旧行为。
        /// </summary>
        public IntPtr CustomBalloonIconHandle;

        /// <summary>
        /// 创建一个默认数据结构，该结构提供隐藏的任务栏图标而不设置该图标。
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        /// <param name="iconFile">图标路径</param>
        /// <param name="toolTip">提示文本</param>
        /// <returns>NOTIFYICONDATA</returns>
        public static NOTIFYICONDATA CreateDefault(IntPtr handle, string iconFile, string toolTip)
        {
            var data = new NOTIFYICONDATA();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                // 使用当前大小
                data.cbSize = (uint)Marshal.SizeOf(data);
            }
            else
            {
                data.cbSize = 952;
                data.VersionOrTimeout = 10;
            }

            data.WindowHandle = handle;
            data.TaskbarIconId = 0x0;
            data.CallbackMessageId = WindowMessageSink.CallbackMessageId;
            data.VersionOrTimeout = (uint)0x4;

            IntPtr hIcon = User32Library.LoadImage(IntPtr.Zero, iconFile,
                ImageType.IMAGE_ICON, 16, 16, LoadImageFlags.LR_LOADFROMFILE);

            data.IconHandle = hIcon;

            //hide initially
            data.IconState = IconState.Hidden;
            data.StateMask = IconState.Hidden;

            //set flags
            data.ValidMembers = IconDataMembers.Message
                                | IconDataMembers.Icon
                                | IconDataMembers.Tip;

            //reset strings
            data.ToolTipText = data.BalloonText = data.BalloonTitle = toolTip;

            return data;
        }
    }
}
