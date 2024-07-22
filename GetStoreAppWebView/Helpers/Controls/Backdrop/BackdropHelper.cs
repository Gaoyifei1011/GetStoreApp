using GetStoreAppWebView.WindowsAPI.ComTypes;
using GetStoreAppWebView.WindowsAPI.PInvoke.Combase;
using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWebView.Helpers.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        public static Lazy<IPropertyValueStatics> PropertyValueStatics { get; } = new(() => GetActivationFactory<IPropertyValueStatics>("Windows.Foundation.PropertyValue", typeof(IPropertyValueStatics).GUID));

        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        public static T GetActivationFactory<T>(string activatableClassId, Guid iid)
        {
            if (!string.IsNullOrEmpty(activatableClassId))
            {
                Marshal.ThrowExceptionForHR(CombaseLibrary.WindowsCreateString(activatableClassId, activatableClassId.Length, out IntPtr stringPtr));
                CombaseLibrary.RoGetActivationFactory(stringPtr, iid, out IntPtr comp);
                return (T)Marshal.GetObjectForIUnknown(comp);
            }
            else
            {
                return default;
            }
        }
    }
}
