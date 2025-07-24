using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;
using WinRT;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DisplayInformation 类实现对应的接口
    /// </summary>
    [GeneratedComInterface, Guid("5586D03C-B4B6-594E-96AD-8372700B08DD")]
    public partial interface IDisplayInformation2 : IInspectable
    {
        /// <summary>
        /// 获取显示监视器每英寸 (DPI) 的原始点数。
        /// </summary>
        /// <param name="rawDpi">每英寸 (DPI) 的原始点数</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetRawDpi(out Point rawDpi);

        /// <summary>
        /// 获取一个值，该值表示每个视图 (布局) 像素的原始 (物理) 像素数。
        /// </summary>
        /// <param name="rawPixelsPerViewPixel">每个视图 (布局) 像素的原始 (物理) 像素数</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetRawPixelsPerViewPixel(out double rawPixelsPerViewPixel);

        /// <summary>
        /// 注册设备的每英寸 (DPI) 的原始点数属性更改时发生的事件。
        /// </summary>
        /// <param name="handler">设备的每英寸 (DPI) 的原始点数属性更改时发生的事件指针</param>
        /// <param name="token">设备的每英寸 (DPI) 的原始点数属性更改时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddDpiChanged(nint handler, out EventRegistrationToken token);

        /// <summary>
        /// 注销设备的每英寸 (DPI) 的原始点数属性更改时发生的事件。
        /// </summary>
        /// <param name="token">设备的每英寸 (DPI) 的原始点数属性更改时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveDpiChanged(EventRegistrationToken token);

        /// <summary>
        /// 获取显示监视器的本机方向，这通常是设备上的按钮与监视器方向匹配的方向。
        /// </summary>
        /// <param name="displayOrientation">与监视器方向匹配的方向</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetAngularOffsetFromNativeOrientation(out int displayOrientation);

        /// <summary>
        /// 注册设备上的按钮与监视器方向匹配的方向更改时发生的事件。
        /// </summary>
        /// <param name="handler">注册设备上的按钮与监视器方向匹配的方向更改时发生的事件指针</param>
        /// <param name="token">注册设备上的按钮与监视器方向匹配的方向更改时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int AddOrientationChanged(nint handler, out EventRegistrationToken token);

        /// <summary>
        /// 注册注销设备上的按钮与监视器方向匹配的方向更改时发生的事件。
        /// </summary>
        /// <param name="token">注册设备上的按钮与监视器方向匹配的方向更改时发生的事件令牌</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int RemoveOrientationChanged(EventRegistrationToken token);
    }
}
