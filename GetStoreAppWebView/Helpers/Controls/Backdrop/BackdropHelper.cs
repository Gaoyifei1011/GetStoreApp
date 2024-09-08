using GetStoreAppWebView.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreAppWebView.Helpers.Controls.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        private static readonly StrategyBasedComWrappers strategyBasedComWrappers = new();

        public static Lazy<IPropertyValueStatics> PropertyValueStatics { get; } = new(() => GetActivationFactory<IPropertyValueStatics>("Windows.Foundation.PropertyValue", typeof(IPropertyValueStatics).GUID));

        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        public static T GetActivationFactory<T>(string activatableClassId, Guid iid)
        {
            if (!string.IsNullOrEmpty(activatableClassId))
            {
                IObjectReference objectReference = ActivationFactory.Get(activatableClassId, iid);
                return (T)strategyBasedComWrappers.GetOrCreateObjectForComInstance(objectReference.ThisPtr, CreateObjectFlags.None);
            }
            else
            {
                return default;
            }
        }
    }
}
