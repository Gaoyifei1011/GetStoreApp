using GetStoreApp.Helpers.Root;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 为了配置而提交的结构任务栏图标。提供各种成员可以部分配置，根据 <see cref="IconDataMembers"/> 中的定义。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct NOTIFYICONDATA
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// 接收与任务栏状态区域中的图标关联的通知消息的窗口句柄。命令行管理程序使用 hWnd 和 uID 来标识调用Shell_NotifyIcon时要操作的图标。
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        /// 任务栏图标的应用程序定义的标识符。命令行管理程序使用 hWnd 和 uID 来标识调用Shell_NotifyIcon时要操作的图标。
        /// 您可以通过为每个图标分配不同的 uID 来将多个图标与单个 hWnd 相关联。但是，此功能目前未使用。
        /// </summary>
        public uint uID;

        /// <summary>
        /// 指示结构的哪些其他成员包含有效数据的标志，或者向工具提示提供有关其显示方式的其他信息。
        /// </summary>
        public NotifyIconFlags uFlags;

        /// <summary>
        /// 应用程序定义的消息标识符。 系统使用此标识符将通知消息发送到 hWnd 中标识的窗口。 当鼠标事件或悬停在图标的边框中发生时，当使用键盘选择或激活图标时，或者当这些操作在气球通知中发生时，将发送这些通知消息。
        /// 当 uVersion 成员为 0 或 NOTIFYICON_VERSION 时，消息的 wParam 参数包含发生事件的任务栏图标的标识符。 此标识符的长度可以是 32 位。 lParam 参数保存与事件关联的鼠标或键盘消息。 例如，当指针移动到任务栏图标上时， lParam 设置为 WM_MOUSEMOVE。
        /// 当 uVersion 成员NOTIFYICON_VERSION_4时，应用程序继续通过 uCallbackMessage 成员以应用程序定义消息的形式接收通知事件，但该消息的 lParam 和 wParam 参数的解释将更改如下：
        /// LOWORD(lParam) 包含通知事件，例如NIN_BALLOONSHOW、NIN_POPUPOPEN或WM_CONTEXTMENU。
        /// HIWORD(lParam) 包含图标 ID。 图标 ID 的长度限制为 16 位。
        /// GET_X_LPARAM(wParam) 返回通知事件NIN_POPUPOPEN、NIN_SELECT、NIN_KEYSELECT以及WM_MOUSEFIRST和WM_MOUSELAST之间的所有鼠标消息的 X 定位点坐标。 如果其中任何消息是由键盘生成的， 则 wParam 将设置为目标图标的左上角。 对于所有其他消息， wParam 未定义。
        /// GET_Y_LPARAM(wParam) 返回为 X 定位点定义的通知事件和消息的 Y 定位点坐标。
        /// </summary>
        public uint uCallbackMessage;

        /// <summary>
        /// 要添加、修改或删除的图标的句柄。 Windows XP 和更高版本支持最多 32 BPP 的图标。
        /// 如果仅提供 16x16 像素图标，则会在设置为高 dpi 值的系统中将其缩放为更大的大小。 这可能会导致无吸引力的结果。 建议在资源文件中同时提供 16x16 像素图标和 32x32 图标。 使用 LoadIconMetric 确保正确加载和缩放正确的图标。
        /// </summary>
        public IntPtr hIcon;

        /// <summary>
        /// 以 null 结尾的字符串，指定标准工具提示的文本。 它最多可以包含 64 个字符，包括终止 null 字符。
        /// 对于 Windows 2000 及更高版本， <see cref="szTip"> 最多可以包含 128 个字符，包括终止 null 字符。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;

        /// <summary>
        /// Windows 2000 及更高版本。 图标的状态
        /// </summary>
        public NotifyIconState dwState;

        /// <summary>
        ///  一个值，该值指定检索或修改 dwState 成员的哪些位。 可能的值与 dwState 的值相同。
        /// </summary>
        public NotifyIconState dwStateMask;

        /// <summary>
        /// 以 null 结尾的字符串，指定要在气球通知中显示的文本。 它最多可以包含 256 个字符，包括终止 null 字符，但应限制为 200 个字符（英语），以适应本地化。 若要从 UI 中删除气球通知，请删除带有 NIM_DELETE) 的图标 (，或在 uFlags 中设置NIF_INFO标志，并将 szInfo 设置为空字符串。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szInfo;

        /// <summary>
        /// 与 uVersion 联合。 通知的超时值（以毫秒为单位）。 系统强制实施最小和最大超时值。 在 uTimeout 中指定的过大值将设置为最大值。 太小的值默认为最小值。 系统最小和最大超时值当前分别设置为 10 秒和 30 秒。 有关 uTimeout 的进一步讨论，请参阅备注。
        /// Windows 2000 及更高版本。 与 uTimeout 的联合 (从 Windows Vista) 开始弃用。 指定应使用哪个版本的 Shell 通知图标界面。 有关这些版本中的差异的详细信息，请参阅 <see cref="Shell32Library.Shell_NotifyIcon">。 仅当使用 <see cref="Shell32Library.Shell_NotifyIcon"> 发送 NIM_SETVERSION 消息时，才使用此成员。
        /// </summary>
        public uint uVersion;

        /// <summary>
        ///  一个以 null 结尾的字符串，指定气球通知的标题。 此标题以紧邻文本上方的大字体显示。 它最多可以包含 64 个字符，包括终止 null 字符，但英语应限制为 48 个字符，以适应本地化。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szInfoTitle;

        /// <summary>
        /// Windows 2000 及更高版本。 可以设置为修改气球通知的行为和外观的标志。 图标放置在标题的左侧。 如果 szInfoTitle 成员的长度为零，则不显示图标。
        /// </summary>
        public NotifyIconInfoFlags dwInfoFlags;

        /// <summary>
        /// Windows XP 及更高版本。
        /// Windows 7 及更高版本：标识图标的已注册 GUID。 此值将替代 uID ，是标识图标的推荐方法。 必须在 uFlags 成员中设置NIF_GUID标志。
        /// Windows XP 和 Windows Vista：保留;必须设置为 0。
        /// 如果应用程序打算在 Windows Vista 和 Windows 7 上运行，则必须检查 Windows 版本，并且仅在 Windows 7 或更高版本上指定非零 guidItem。
        /// 如果在一次调用 <see cref="Shell32Library.Shell_NotifyIcon"> 时使用 GUID 标识通知图标，则必须在处理同一图标的任何后续 <see cref="Shell32Library.Shell_NotifyIcon"> 调用中使用相同的 GUID 来标识该图标。
        /// </summary>
        public Guid guidItem;

        /// <summary>
        /// Windows Vista 及更高版本。 应用程序提供的自定义通知图标的句柄，应独立于通知区域图标使用。 如果此成员为非 NULL，并在 <see cref="dwInfoFlags"> 成员中设置了 NIIF_USER 标志，则此图标将用作通知图标。 如果此成员为 NULL，则执行旧行为。
        /// </summary>
        public IntPtr hBalloonIcon;

        /// <summary>
        /// 创建<see cref="NOTIFYICONDATA">结构体
        /// </summary>
        public static NOTIFYICONDATA Initialize(IntPtr hWnd, [Optional, DefaultParameterValue("")] string toolTip)
        {
            NOTIFYICONDATA data = new NOTIFYICONDATA();

            if (InfoHelper.GetSystemVersion().Major >= 6)
            {
                data.cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA));
            }
            else
            {
                data.cbSize = 952;
                data.uVersion = 10;
            }

            data.hWnd = hWnd;
            data.uID = 0x0;
            data.uCallbackMessage = TrayMessageWindow.CallbackMessageId;
            data.uVersion = 0x4;
            data.hIcon = LoadIcon();
            data.dwState = NotifyIconState.NIS_HIDDEN;
            data.dwStateMask = NotifyIconState.NIS_HIDDEN;
            data.uFlags = NotifyIconFlags.NIF_MESSAGE | NotifyIconFlags.NIF_ICON | NotifyIconFlags.NIF_TIP;
            data.szTip = toolTip;
            return data;
        }

        /// <summary>
        /// 从exe应用程序中加载图标文件
        /// </summary>
        private static IntPtr LoadIcon()
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe"), 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            IntPtr[] hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe"), 0, 16, 16, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标，返回该应用程序的图标句柄
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                return hIcons[0];
            }
            else
            {
                return IntPtr.Zero;
            }
        }
    }
}
