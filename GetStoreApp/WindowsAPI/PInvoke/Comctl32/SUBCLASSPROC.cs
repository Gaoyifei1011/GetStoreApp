using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    /// <summary>
    /// 定义 RemoveWindowSubclass 和 SetWindowSubclass 使用的回调函数的原型。
    /// </summary>
    /// <param name="hWnd">子类化窗口的句柄。</param>
    /// <param name="uMsg">正在传递的消息。</param>
    /// <param name="wParam">其他消息信息。 此参数的内容取决于 uMsg 的值。</param>
    /// <param name="lParam">其他消息信息。 此参数的内容取决于 uMsg 的值。</param>
    /// <param name="uIdSubclass">子类 ID。</param>
    /// <param name="dwRefData">提供给 SetWindowSubclass 函数的引用数据。 这可用于将子类实例与“this”指针相关联。</param>
    /// <returns>返回值是消息处理的结果，取决于发送的消息。</returns>
    public delegate IntPtr SUBCLASSPROC(IntPtr hWnd, WindowMessage uMsg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData);
}
