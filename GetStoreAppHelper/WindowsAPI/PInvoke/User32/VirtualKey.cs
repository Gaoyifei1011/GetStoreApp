using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    /// <summary>虚拟键代码</summary>
    /// <remarks>在Windows SDK v6.1的winuser.h中定义。</remarks>
    /// <devremarks>
    /// API对虚拟键的参数期望的长度各不相同。
    /// 请确保适当地将参数类型设置为byte、ushort或int，并记录用户应该从这个枚举中获取值并强制转换结果，以确保方法签名是兼容的。
    /// </devremarks>
    public enum VirtualKey : ushort
    {
        /// <summary>
        /// 这是一个附录，用于必须传递零值以表示无键代码的函数。
        /// </summary>
        VK_NO_KEY = 0,

        /// <summary>
        /// 鼠标左键
        /// </summary>
        VK_LBUTTON = 0x01,

        /// <summary>
        /// 鼠标右键
        /// </summary>
        VK_RBUTTON = 0x02,

        /// <summary>
        /// 控制中断处理
        /// </summary>
        VK_CANCEL = 0x03,

        /// <summary>
        /// 中间鼠标按钮 (三键鼠标)
        /// </summary>
        /// <remarks>L和R键不相邻</remarks>
        VK_MBUTTON = 0x04,

        /// <summary>
        /// X1 鼠标按钮
        /// </summary>
        /// <remarks>L和R键不相邻</remarks>
        VK_XBUTTON1 = 0x05,

        /// <summary>
        /// X2 鼠标按钮
        /// </summary>
        /// <remarks>L和R键不相邻</remarks>
        VK_XBUTTON2 = 0x06,

        /* 0x07 : 未定义 */

        /// <summary>
        /// BACKSPACE 键
        /// </summary>
        VK_BACK = 0x08,

        /// <summary>
        /// Tab 键
        /// </summary>
        VK_TAB = 0x09,

        /* 0x0A - 0x0B : 预留 */

        /// <summary>
        ///  CLEAR 键
        /// </summary>
        VK_CLEAR = 0x0C,

        /// <summary>
        /// Enter 键
        /// </summary>
        VK_RETURN = 0x0D,

        /* 0x0E - 0x0F : 未定义 */

        /// <summary>
        /// SHIFT 键
        /// </summary>
        VK_SHIFT = 0x10,

        /// <summary>
        ///  Ctrl 键
        /// </summary>
        VK_CONTROL = 0x11,

        /// <summary>
        /// Alt 键
        /// </summary>
        VK_MENU = 0x12,

        /// <summary>
        /// PAUSE 键
        /// </summary>
        VK_PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK 键
        /// </summary>
        VK_CAPITAL = 0x14,

        /// <summary>
        /// IME Kana 模式
        /// </summary>
        VK_KANA = 0x15,

        /// <summary>
        /// IME 朝鲜文库埃尔模式 (保持兼容性;使用 <see crf="VK_HANGUL">)
        /// </summary>
        [Obsolete("Use VK_HANGUL instead")]
        VK_HANGEUL = 0x15,

        /// <summary>
        /// IME Hanguel 模式
        /// </summary>
        VK_HANGUL = 0x15,

        /// <summary>
        /// IME On
        /// </summary>
        VK_IME_ON = 0x16,

        /// <summary>
        /// IME Junja 模式
        /// </summary>
        VK_JUNJA = 0x17,

        /// <summary>
        /// IME 最终模式
        /// </summary>
        VK_FINAL = 0x18,

        /// <summary>
        /// IME Hanja 模式
        /// </summary>
        VK_HANJA = 0x19,

        /// <summary>
        /// IME Kanji 模式
        /// </summary>
        VK_KANJI = 0x19,

        /// <summary>
        /// IME 关闭
        /// </summary>
        VK_IME_OFF = 0x1A,

        /// <summary>
        /// ESC 键
        /// </summary>
        VK_ESCAPE = 0x1B,

        /// <summary>
        /// IME 转换
        /// </summary>
        VK_CONVERT = 0x1C,

        /// <summary>
        /// IME 不转换
        /// </summary>
        VK_NONCONVERT = 0x1D,

        /// <summary>
        /// IME 接受
        /// </summary>
        VK_ACCEPT = 0x1E,

        /// <summary>
        /// IME 模式更改请求
        /// </summary>
        VK_MODECHANGE = 0x1F,

        /// <summary>
        /// 空格键
        /// </summary>
        VK_SPACE = 0x20,

        /// <summary>
        /// PAGE UP 键
        /// </summary>
        VK_PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN 键
        /// </summary>
        VK_NEXT = 0x22,

        /// <summary>
        /// END 键
        /// </summary>
        VK_END = 0x23,

        /// <summary>
        /// HOME 键
        /// </summary>
        VK_HOME = 0x24,

        /// <summary>
        /// 向左键
        /// </summary>
        VK_LEFT = 0x25,

        /// <summary>
        /// 向上键
        /// </summary>
        VK_UP = 0x26,

        /// <summary>
        /// 向右键
        /// </summary>
        VK_RIGHT = 0x27,

        /// <summary>
        /// 向下键
        /// </summary>
        VK_DOWN = 0x28,

        /// <summary>
        /// SELECT 键
        /// </summary>
        VK_SELECT = 0x29,

        /// <summary>
        /// PRINT 键
        /// </summary>
        VK_PRINT = 0x2A,

        /// <summary>
        /// EXECUTE 键
        /// </summary>
        VK_EXECUTE = 0x2B,

        /// <summary>
        /// 打印屏幕键
        /// </summary>
        VK_SNAPSHOT = 0x2C,

        /// <summary>
        /// INS 键
        /// </summary>
        VK_INSERT = 0x2D,

        /// <summary>
        /// DEL 键
        /// </summary>
        VK_DELETE = 0x2E,

        /// <summary>
        /// 帮助键
        /// </summary>
        VK_HELP = 0x2F,

        /// <summary>
        /// 0 键
        /// </summary>
        VK_KEY_0 = 0x30,

        /// <summary>
        /// 1 键
        /// </summary>
        VK_KEY_1 = 0x31,

        /// <summary>
        /// 2 键
        /// </summary>
        VK_KEY_2 = 0x32,

        /// <summary>
        /// 3 键
        /// </summary>
        VK_KEY_3 = 0x33,

        /// <summary>
        /// 4 键
        /// </summary>
        VK_KEY_4 = 0x34,

        /// <summary>
        /// 5 键
        /// </summary>
        VK_KEY_5 = 0x35,

        /// <summary>
        /// 6 键
        /// </summary>
        VK_KEY_6 = 0x36,

        /// <summary>
        /// 7 键
        /// </summary>
        VK_KEY_7 = 0x37,

        /// <summary>
        /// 8 键
        /// </summary>
        VK_KEY_8 = 0x38,

        /// <summary>
        /// 9 键
        /// </summary>
        VK_KEY_9 = 0x39,

        /* 0x3A - 0x40 : 未定义 */

        /// <summary>
        /// A 键
        /// </summary>
        VK_A = 0x41,

        /// <summary>
        /// B 键
        /// </summary>
        VK_B = 0x42,

        /// <summary>
        /// C 键
        /// </summary>
        VK_C = 0x43,

        /// <summary>
        /// D 键
        /// </summary>
        VK_D = 0x44,

        /// <summary>
        /// E 键
        /// </summary>
        VK_E = 0x45,

        /// <summary>
        /// F 键
        /// </summary>
        VK_F = 0x46,

        /// <summary>
        /// G 键
        /// </summary>
        VK_G = 0x47,

        /// <summary>
        /// H 键
        /// </summary>
        VK_H = 0x48,

        /// <summary>
        /// I 键
        /// </summary>
        VK_I = 0x49,

        /// <summary>
        /// J 键
        /// </summary>
        VK_J = 0x4A,

        /// <summary>
        /// K 键
        /// </summary>
        VK_K = 0x4B,

        /// <summary>
        /// L 键
        /// </summary>
        VK_L = 0x4C,

        /// <summary>
        /// M 键
        /// </summary>
        VK_M = 0x4D,

        /// <summary>
        /// N 键
        /// </summary>
        VK_N = 0x4E,

        /// <summary>
        /// O 键
        /// </summary>
        VK_O = 0x4F,

        /// <summary>
        /// P 键
        /// </summary>
        VK_P = 0x50,

        /// <summary>
        /// Q 键
        /// </summary>
        VK_Q = 0x51,

        /// <summary>
        /// R 键
        /// </summary>
        VK_R = 0x52,

        /// <summary>
        /// S 键
        /// </summary>
        VK_S = 0x53,

        /// <summary>
        /// T 键
        /// </summary>
        VK_T = 0x54,

        /// <summary>
        /// U 键
        /// </summary>
        VK_U = 0x55,

        /// <summary>
        /// V 键
        /// </summary>
        VK_V = 0x56,

        /// <summary>
        /// W 键
        /// </summary>
        VK_W = 0x57,

        /// <summary>
        /// X 键
        /// </summary>
        VK_X = 0x58,

        /// <summary>
        /// Y 键
        /// </summary>
        VK_Y = 0x59,

        /// <summary>
        /// Z 键
        /// </summary>
        VK_Z = 0x5A,

        /// <summary>
        /// 左Windows键 (自然键盘)
        /// </summary>
        VK_LWIN = 0x5B,

        /// <summary>
        /// 右Windows键 (自然键盘)
        /// </summary>
        VK_RWIN = 0x5C,

        /// <summary>
        /// 应用程序键 (自然键盘)
        /// </summary>
        VK_APPS = 0x5D,

        /* 0x5E : 保留 */

        /// <summary>
        /// 计算机休眠键
        /// </summary>
        VK_SLEEP = 0x5F,

        /// <summary>
        /// 数字键盘 0 键
        /// </summary>
        VK_NUMPAD0 = 0x60,

        /// <summary>
        /// 数字键盘 1 键
        /// </summary>
        VK_NUMPAD1 = 0x61,

        /// <summary>
        /// 数字键盘 2 键
        /// </summary>
        VK_NUMPAD2 = 0x62,

        /// <summary>
        /// 数字键盘 3 键
        /// </summary>
        VK_NUMPAD3 = 0x63,

        /// <summary>
        /// 数字键盘 4 键
        /// </summary>
        VK_NUMPAD4 = 0x64,

        /// <summary>
        /// 数字键盘 5 键
        /// </summary>
        VK_NUMPAD5 = 0x65,

        /// <summary>
        /// 数字键盘 6 键
        /// </summary>
        VK_NUMPAD6 = 0x66,

        /// <summary>
        /// 数字键盘 7 键
        /// </summary>
        VK_NUMPAD7 = 0x67,

        /// <summary>
        /// 数字键盘 8 键
        /// </summary>
        VK_NUMPAD8 = 0x68,

        /// <summary>
        /// 数字键盘 9 键
        /// </summary>
        VK_NUMPAD9 = 0x69,

        /// <summary>
        /// 乘键
        /// </summary>
        VK_MULTIPLY = 0x6A,

        /// <summary>
        /// 添加键
        /// </summary>
        VK_ADD = 0x6B,

        /// <summary>
        /// 分隔符键
        /// </summary>
        VK_SEPARATOR = 0x6C,

        /// <summary>
        /// 减去键
        /// </summary>
        VK_SUBTRACT = 0x6D,

        /// <summary>
        /// 十进制键
        /// </summary>
        VK_DECIMAL = 0x6E,

        /// <summary>
        /// 除键
        /// </summary>
        VK_DIVIDE = 0x6F,

        /// <summary>
        /// F1 键
        /// </summary>
        VK_F1 = 0x70,

        /// <summary>
        /// F2 键
        /// </summary>
        VK_F2 = 0x71,

        /// <summary>
        /// F3 键
        /// </summary>
        VK_F3 = 0x72,

        /// <summary>
        /// F4 键
        /// </summary>
        VK_F4 = 0x73,

        /// <summary>
        /// F5 键
        /// </summary>
        VK_F5 = 0x74,

        /// <summary>
        /// F6 键
        /// </summary>
        VK_F6 = 0x75,

        /// <summary>
        /// F7 键
        /// </summary>
        VK_F7 = 0x76,

        /// <summary>
        /// F8 键
        /// </summary>
        VK_F8 = 0x77,

        /// <summary>
        /// F9 键
        /// </summary>
        VK_F9 = 0x78,

        /// <summary>
        /// F10 键
        /// </summary>
        VK_F10 = 0x79,

        /// <summary>
        /// F11 键
        /// </summary>
        VK_F11 = 0x7A,

        /// <summary>
        /// F12 键
        /// </summary>
        VK_F12 = 0x7B,

        /// <summary>
        /// F13 键
        /// </summary>
        VK_F13 = 0x7C,

        /// <summary>
        /// F14 键
        /// </summary>
        VK_F14 = 0x7D,

        /// <summary>
        /// F15 键
        /// </summary>
        VK_F15 = 0x7E,

        /// <summary>
        /// F16 键
        /// </summary>
        VK_F16 = 0x7F,

        /// <summary>
        /// F17 键
        /// </summary>
        VK_F17 = 0x80,

        /// <summary>
        /// F18 键
        /// </summary>
        VK_F18 = 0x81,

        /// <summary>
        /// F19 键
        /// </summary>
        VK_F19 = 0x82,

        /// <summary>
        /// F20 键
        /// </summary>
        VK_F20 = 0x83,

        /// <summary>
        /// F21 键
        /// </summary>
        VK_F21 = 0x84,

        /// <summary>
        /// F22 键
        /// </summary>
        VK_F22 = 0x85,

        /// <summary>
        /// F23 键
        /// </summary>
        VK_F23 = 0x86,

        /// <summary>
        /// F24 键
        /// </summary>
        VK_F24 = 0x87,

        /* 0x88 - 0x8F : 未定义 */

        /// <summary>
        /// NUM LOCK 键
        /// </summary>
        VK_NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK 键
        /// </summary>
        VK_SCROLL = 0x91,

        /* 0x92 - 0x96 : OEM 特定 */

        /// <summary>
        /// numpad上的'='键 (NEC PC-9800 kbd定义)
        /// </summary>
        VK_OEM_NEC_EQUAL = 0x92,

        /// <summary>
        /// “Dictionary”键 (Fujitsu/OASYS kbd定义)
        /// </summary>
        VK_OEM_FJ_JISHO = 0x92,

        /// <summary>
        /// “Unregister word”键 (Fujitsu/OASYS kbd定义)
        /// </summary>
        VK_OEM_FJ_MASSHOU = 0x93,

        /// <summary>
        /// 'Register word' 键 (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_TOUROKU = 0x94,

        /// <summary>
        /// 'Left OYAYUBI' 键 (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_LOYA = 0x95,

        /// <summary>
        /// 'Right OYAYUBI' 键 (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_ROYA = 0x96,

        /* 0x97 - 0x9F : 未定义 */

        /// <summary>
        /// 左 SHIFT 键
        /// </summary>
        /// <remarks>Used only as parameters to <see cref="GetAsyncKeyState" /> and  <see cref="GetKeyState" />. * No other API or message will distinguish left and right keys in this way.</remarks>
        VK_LSHIFT = 0xA0,

        /// <summary>
        /// 右 SHIFT 键
        /// </summary>
        VK_RSHIFT = 0xA1,

        /// <summary>
        /// 左 Ctrl 键
        /// </summary>
        VK_LCONTROL = 0xA2,

        /// <summary>
        /// 右 Ctrl 键
        /// </summary>
        VK_RCONTROL = 0xA3,

        /// <summary>
        /// 左 Alt 键
        /// </summary>
        VK_LMENU = 0xA4,

        /// <summary>
        /// 右 Alt 键
        /// </summary>
        VK_RMENU = 0xA5,

        /// <summary>
        /// 浏览器后退键
        /// </summary>
        VK_BROWSER_BACK = 0xA6,

        /// <summary>
        /// 浏览器前进键
        /// </summary>
        VK_BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// 浏览器刷新键
        /// </summary>
        VK_BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// 浏览器停止键
        /// </summary>
        VK_BROWSER_STOP = 0xA9,

        /// <summary>
        /// 浏览器搜索键
        /// </summary>
        VK_BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// 浏览器收藏键
        /// </summary>
        VK_BROWSER_FAVORITES = 0xAB,

        /// <summary>
        /// 浏览器“开始”和“主页”键
        /// </summary>
        VK_BROWSER_HOME = 0xAC,

        /// <summary>
        /// 静音键
        /// </summary>
        VK_VOLUME_MUTE = 0xAD,

        /// <summary>
        /// 音量减小键
        /// </summary>
        VK_VOLUME_DOWN = 0xAE,

        /// <summary>
        /// 音量增加键
        /// </summary>
        VK_VOLUME_UP = 0xAF,

        /// <summary>
        /// 下一曲目键
        /// </summary>
        VK_MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// 上一曲目键
        /// </summary>
        VK_MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// 停止媒体键
        /// </summary>
        VK_MEDIA_STOP = 0xB2,

        /// <summary>
        /// 播放/暂停媒体键
        /// </summary>
        VK_MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>
        /// 启动邮件键
        /// </summary>
        VK_LAUNCH_MAIL = 0xB4,

        /// <summary>
        /// 选择媒体键
        /// </summary>
        VK_LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>
        /// 启动应用程序 1 键
        /// </summary>
        VK_LAUNCH_APP1 = 0xB6,

        /// <summary>
        /// 启动应用程序 2 键
        /// </summary>
        VK_LAUNCH_APP2 = 0xB7,

        /* 0xB8 - 0xB9 : 预留 */

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“;：”键</remarks>
        VK_OEM_1 = 0xBA,

        /// <summary>
        /// 对于任何国家/地区，“+”键
        /// </summary>
        VK_OEM_PLUS = 0xBB,

        /// <summary>
        /// 对于任何国家/地区，“，键
        /// </summary>
        VK_OEM_COMMA = 0xBC,

        /// <summary>
        /// 对于任何国家/地区，“-”键
        /// </summary>
        VK_OEM_MINUS = 0xBD,

        /// <summary>
        /// 对于任何国家/地区，“.”键
        /// </summary>
        VK_OEM_PERIOD = 0xBE,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“/？” key</remarks>
        VK_OEM_2 = 0xBF,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“~”键</remarks>
        VK_OEM_3 = 0xC0,

        /* 0xC1 - 0xD7 : 预留 */
        /* 0xD8 - 0xDA : 未定义 */

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“[{”键</remarks>
        VK_OEM_4 = 0xDB,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“\|”键</remarks>
        VK_OEM_5 = 0xDC,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“]}”键</remarks>
        VK_OEM_6 = 0xDD,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        /// <remarks>对于美国标准键盘，“单引号/双引号”键</remarks>
        VK_OEM_7 = 0xDE,

        /// <summary>
        /// 用于其他字符;它可能因键盘而异。
        /// </summary>
        VK_OEM_8 = 0xDF,

        /* 0xE0 : 保留 */

        /* 0xE1 : OEM 特定 */

        /// <summary>
        /// OEM 特定
        /// </summary>
        /// <remarks>'AX'键在日本AX kbd</remarks>
        VK_OEM_AX = 0xE1,

        /// <summary>
        /// <>美国标准键盘上的键，或\\|非美国 102 键键盘上的键
        /// </summary>
        VK_OEM_102 = 0xE2,

        /* 0xE3 - 0xE4 : OEM 特定 */

        /// <summary>
        /// OEM 特定
        /// </summary>
        /// <remarks>ICO帮助键</remarks>
        VK_ICO_HELP = 0xE3,

        /// <summary>
        /// OEM 特定
        /// </summary>
        /// <remarks>00键开启ICO</remarks>
        VK_ICO_00 = 0xE4,

        /// <summary>
        /// IME PROCESS 键
        /// </summary>
        VK_PROCESSKEY = 0xE5,

        /* 0xE6 : OEM 特定 */

        /// <summary>
        /// OEM 特定
        /// </summary>
        /// <remarks>清除ICO的键</remarks>
        VK_ICO_CLEAR = 0xE6,

        /// <summary>
        /// 用于将 Unicode 字符当作键击传递。 该 VK_PACKET 键是用于非键盘输入方法的 32 位虚拟键值的低字。
        /// </summary>
        /// <remarks>有关详细信息，请参阅“备注”，以及 <see cref="KEYBDINPUT"/>, <see cref="SendInput(int, INPUT*, int)"/>, <see cref="WindowMessage.WM_KEYDOWN"/>, 和 <see cref="WindowMessage.WM_KEYUP"/></remarks>
        VK_PACKET = 0xE7,

        /* 0xE8 : 未定义 */

        /* 0xE9 - 0xF5 : OEM 特定 */

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_RESET = 0xE9,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_JUMP = 0xEA,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_PA1 = 0xEB,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_PA2 = 0xEC,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_PA3 = 0xED,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_WSCTRL = 0xEE,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_CUSEL = 0xEF,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_ATTN = 0xF0,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_FINISH = 0xF1,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_COPY = 0xF2,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_AUTO = 0xF3,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_ENLW = 0xF4,

        /// <summary>
        /// 诺基亚/爱立信的定义
        /// </summary>
        VK_OEM_BACKTAB = 0xF5,

        /// <summary>
        /// Attn 键
        /// </summary>
        VK_ATTN = 0xF6,

        /// <summary>
        /// CrSel 键
        /// </summary>
        VK_CRSEL = 0xF7,

        /// <summary>
        /// ExSel 键
        /// </summary>
        VK_EXSEL = 0xF8,

        /// <summary>
        /// 擦除 EOF 键
        /// </summary>
        VK_EREOF = 0xF9,

        /// <summary>
        /// 播放键
        /// </summary>
        VK_PLAY = 0xFA,

        /// <summary>
        /// 缩放键
        /// </summary>
        VK_ZOOM = 0xFB,

        /// <summary>
        /// Windows头文件定义保留的常量
        /// </summary>
        VK_NONAME = 0xFC,

        /// <summary>
        /// PA1 键
        /// </summary>
        VK_PA1 = 0xFD,

        /// <summary>
        /// 清除键
        /// </summary>
        VK_OEM_CLEAR = 0xFE,

        /* 0xFF : 预留 */
    }
}
