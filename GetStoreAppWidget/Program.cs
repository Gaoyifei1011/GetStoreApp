using GetStoreAppWidget.WindowsAPI.PInvoke.Ole32;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppWidget
{
    /// <summary>
    /// 获取商店应用 小组件
    /// </summary>
    public class Program
    {
        private static readonly WidgetProviderFactory<WidgetProvider> widgetProviderFactory = new();
        private static AutoResetEvent autoResetEvent = new(false);

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        public static unsafe void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            Ole32Library.CoRegisterClassObject(typeof(WidgetProvider).GUID, (IntPtr)ComInterfaceMarshaller<WidgetProviderFactory<WidgetProvider>>.ConvertToUnmanaged(widgetProviderFactory), CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE, out uint registrationHandle);
            autoResetEvent.WaitOne();
            autoResetEvent.Dispose();
            autoResetEvent = null;
            Ole32Library.CoRevokeClassObject(registrationHandle);
        }

        /// <summary>
        /// 关闭小组件
        /// </summary>
        public static void CloseWidget()
        {
            autoResetEvent?.Set();
        }
    }
}
