using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using WinRT;

namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("C12779D8-85D2-43E5-901A-95DD4F8ECBA3")]
    public partial interface IInternalCoreWindow2
    {
        /// <summary>
        /// 获取由当前Windows 运行时类实现的接口。
        /// </summary>
        /// <param name="iidCount">当前 Windows 运行时 对象实现的接口数，不包括 IUnknown 和 IInspectable 实现。</param>
        /// <param name="iids">指向数组的指针，该数组包含当前 Windows 运行时 对象实现的每个接口的 IID。 排除 IUnknown 和 IInspectable 接口。</param>
        /// <returns>此函数可以返回以下值。S_OK 和 E_OUTOFMEMORY</returns>
        [PreserveSig]
        int GetIids(out ulong iidCount, out IntPtr iids);

        /// <summary>
        /// 获取当前Windows 运行时 对象的完全限定名称。
        /// </summary>
        /// <param name="className">当前Windows 运行时对象的完全限定名称。</param>
        /// <returns>此函数可以返回以下值。S_OK、E_OUTOFMEMORY 和 E_ILLEGAL_METHOD_CALL</returns>
        [PreserveSig]
        int GetRuntimeClassName(out IntPtr className);

        /// <summary>
        /// 获取当前Windows 运行时对象的信任级别。
        /// </summary>
        /// <param name="trustLevel">当前Windows 运行时对象的信任级别。 默认值为 BaseLevel。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetTrustLevel(out TrustLevel trustLevel);

        [PreserveSig]
        int GetLayoutBounds(out Rect rect);

        [PreserveSig]
        int GetVisibleBounds(out Rect rect);

        [PreserveSig]
        int GetDesiredBoundsMode(out ApplicationViewBoundsMode applicationViewBoundsMode);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool SetDesiredBoundsMode(ApplicationViewBoundsMode desiredBoundsMode);

        [PreserveSig]
        int OnVisibleBoundsChange();

        [PreserveSig]
        int AddLayoutBoundsChanged(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveLayoutBoundsChanged(EventRegistrationToken token);

        [PreserveSig]
        int AddVisibleBoundsChanged(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveVisibleBoundsChanged(EventRegistrationToken token);

        [PreserveSig]
        int AddSysKeyDown(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveSysKeyDown(EventRegistrationToken token);

        [PreserveSig]
        int AddSysKeyUp(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveSysKeyUp(EventRegistrationToken token);

        [PreserveSig]
        int AddWindowPositionChanged(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveWindowPositionChanged(EventRegistrationToken token);

        [PreserveSig]
        int AddSettingsChanged(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveSettingsChanged(EventRegistrationToken token);

        [PreserveSig]
        int AddViewStateChanged(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveViewStateChanged(EventRegistrationToken token);

        [PreserveSig]
        int AddDestroying(IntPtr handler, out EventRegistrationToken token);

        [PreserveSig]
        int RemoveDestroying(EventRegistrationToken token);
    }
}
