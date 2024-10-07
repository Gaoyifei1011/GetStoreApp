using GetStoreAppWidget.WindowsAPI.ComTypes;
using Microsoft.Windows.Widgets.Providers;
using System;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreAppWidget
{
    /// <summary>
    /// 允许创建对象的类
    /// </summary>
    [GeneratedComClass]
    public partial class WidgetProviderFactory : IClassFactory
    {
        private readonly IWidgetProvider widgetProvider = new WidgetProvider();

        public int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                ppvObject = IntPtr.Zero;
                return unchecked((int)0x80040110);
            }

            ppvObject = MarshalInspectable<IWidgetProvider>.FromManaged(widgetProvider);
            return 0;
        }

        int IClassFactory.LockServer(bool fLock)
        {
            return 0;
        }
    }
}
