using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using WinRT;

namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("C12779D8-85D2-43E5-901A-95DD4F8ECBA3")]
    public partial interface IInternalCoreWindow2 : IInspectable
    {
        /// <summary>
        /// 获取布局窗口的可见区域
        /// </summary>
        /// <param name="rect">布局窗口的可见区域</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetLayoutBounds(out Rect rect);

        /// <summary>
        /// 获取应用视图（窗口的可见区域）。可见区域是不受部件（如状态栏和应用栏）遮挡的区域。
        /// </summary>
        /// <param name="applicationViewBoundsMode">应用视图 (窗口的可见区域)。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetVisibleBounds(out Rect rect);

        /// <summary>
        /// 获取一个值，该值指示框架用于在应用视图（布局窗口内容的边界）。
        /// </summary>
        /// <param name="applicationViewBoundsMode">窗口的当前布局边界。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetDesiredBoundsMode(out ApplicationViewBoundsMode applicationViewBoundsMode);

        /// <summary>
        /// 设置一个值，该值指示框架用于在应用视图（布局窗口内容的边界）。
        /// </summary>
        /// <param name="applicationViewBoundsMode">窗口的当前布局边界。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool SetDesiredBoundsMode(ApplicationViewBoundsMode desiredBoundsMode);

        /// <summary>
        /// 通知可见区域发生更改
        /// </summary>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int OnVisibleBoundsChange();

        /// <summary>
        /// 注册当 LayoutBounds 的值更改时引发的事件。通常是由于布局窗口的可见区域发生更改的结果。
        /// </summary>
        /// <param name="handler">LayoutBounds 值更改时引发事件的指针</param>
        /// <param name="token">LayoutBounds 值更改时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddLayoutBoundsChanged(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除当 LayoutBounds 的值更改时引发的事件。
        /// </summary>
        /// <param name="token">LayoutBounds 值更改时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveLayoutBoundsChanged(EventRegistrationToken token);

        /// <summary>
        /// 注册当 VisibleBounds 的值更改时引发的事件。通常是由于显示或隐藏状态栏、应用栏或其他部件版式的结果。
        /// </summary>
        /// <param name="handler">VisibleBounds 值更改时引发事件的指针</param>
        /// <param name="token">VisibleBounds 值更改时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddVisibleBoundsChanged(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除当 VisibleBounds 的值更改时引发的事件。
        /// </summary>
        /// <param name="token">VisibleBounds 值更改时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveVisibleBoundsChanged(EventRegistrationToken token);

        /// <summary>
        /// 注册按下非系统键时引发的事件。
        /// </summary>
        /// <param name="handler">按下非系统键时引发事件的指针</param>
        /// <param name="token">按下非系统键时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddSysKeyDown(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除按下非系统键时引发的事件。
        /// </summary>
        /// <param name="token">按下非系统键时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveSysKeyDown(EventRegistrationToken token);

        /// <summary>
        /// 注册在按下非系统键后释放时引发的事件。
        /// </summary>
        /// <param name="handler">按下非系统键后释放时引发事件的指针</param>
        /// <param name="token">按下非系统键后释放时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddSysKeyUp(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除在按下非系统键后释放时引发的事件。
        /// </summary>
        /// <param name="token">在按下非系统键后释放时引发事件的注册令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveSysKeyUp(EventRegistrationToken token);

        /// <summary>
        /// 注册窗口位置发生变化的事件
        /// </summary>
        /// <param name="handler">窗口位置发生变化的事件指针</param>
        /// <param name="token">窗口位置发生变化的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddWindowPositionChanged(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除窗口位置发生变化的事件
        /// </summary>
        /// <param name="token">窗口位置发生变化的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveWindowPositionChanged(EventRegistrationToken token);

        /// <summary>
        /// 注册系统设置发生变化的事件
        /// </summary>
        /// <param name="handler">系统设置发生变化的事件指针</param>
        /// <param name="token">系统设置发生变化的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddSettingsChanged(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除系统设置发生变化的事件
        /// </summary>
        /// <param name="token">系统设置发生变化的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveSettingsChanged(EventRegistrationToken token);

        /// <summary>
        /// 注册指示视图发生改变时的事件
        /// </summary>
        /// <param name="handler">指示视图发生改变时的事件指针</param>
        /// <param name="token">指示视图发生改变时的令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddViewStateChanged(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除指示视图发生改变时的事件
        /// </summary>
        /// <param name="token">指示视图发生改变时的令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveViewStateChanged(EventRegistrationToken token);

        /// <summary>
        /// 注册在销毁窗口时发生的事件。
        /// </summary>
        /// <param name="handler">在销毁窗口时发生的事件指针</param>
        /// <param name="token">在销毁窗口时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddDestroying(IntPtr handler, out EventRegistrationToken token);

        /// <summary>
        /// 移除在销毁窗口时发生的事件。
        /// </summary>
        /// <param name="token">在销毁窗口时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveDestroying(EventRegistrationToken token);
    }
}
