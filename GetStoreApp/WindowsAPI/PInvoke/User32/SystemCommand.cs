namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 请求的系统命令的类型
    /// </summary>
    public enum SYSTEMCOMMAND : int
    {
        /// <summary>
        /// 指示屏幕保存程序是否安全。
        /// </summary>
        SCF_ISSECURE = 0x00000001,

        /// <summary>
        /// 调整窗口的大小。
        /// </summary>
        SC_SIZE = 0xF000,

        /// <summary>
        /// 移动窗口。
        /// </summary>
        SC_MOVE = 0xF010,

        /// <summary>
        /// 最小化窗口。
        /// </summary>
        SC_MINIMIZE = 0xF020,

        /// <summary>
        /// 最大化窗口。
        /// </summary>
        SC_MAXIMIZE = 0xF030,

        /// <summary>
        /// 移动到下一个窗口。
        /// </summary>
        SC_NEXTWINDOW = 0xF040,

        /// <summary>
        /// 移动到上一个窗口。
        /// </summary>
        SC_PREVWINDOW = 0xF050,

        /// <summary>
        /// 关闭窗口。
        /// </summary>
        SC_CLOSE = 0xF060,

        /// <summary>
        /// 垂直滚动。
        /// </summary>
        SC_VSCROLL = 0xF070,

        /// <summary>
        /// 水平滚动。
        /// </summary>
        SC_HSCROLL = 0xF080,

        /// <summary>
        /// 在单击鼠标后检索窗口菜单。
        /// </summary>
        SC_MOUSEMENU = 0xF090,

        /// <summary>
        /// 检索窗口菜单作为击键的结果.
        /// </summary>
        SC_KEYMENU = 0xF100,

        /// <summary>
        /// 将窗口还原到其正常位置和大小。
        /// </summary>
        SC_RESTORE = 0xF120,

        /// <summary>
        /// 激活"开始"菜单菜单。
        /// </summary>
        SC_TASKLIST = 0xF130,

        /// <summary>
        /// 执行在System.ini文件的 [boot] 节中指定的屏幕保存程序应用程序。
        /// </summary>
        SC_SCREENSAVE = 0xF140,

        /// <summary>
        /// 激活与应用程序指定的热键关联的窗口。 lParam 参数标识要激活的窗口。
        /// </summary>
        SC_HOTKEY = 0xF150,

        /// <summary>
        /// 选择默认项;用户双击窗口菜单。
        /// </summary>
        SC_DEFAULT = 0xF160,

        /// <summary>
        /// 设置显示的状态。 此命令支持具有节能功能的设备，例如电池供电的个人电脑。
        /// lParam 参数可以具有以下值：-1 (显示器开机)，1 (显示器将低功率)，2 (显示器正在关闭)
        /// </summary>
        SC_MONITORPOWER = 0xF170,

        /// <summary>
        /// 使用指针将光标更改为问号。 如果用户随后单击对话框中的控件，该控件将收到 WM_HELP 消息。
        /// </summary>
        SC_CONTEXTHELP = 0xF180,
    }
}
