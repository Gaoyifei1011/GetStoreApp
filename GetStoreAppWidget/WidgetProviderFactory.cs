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
    public partial class WidgetProviderFactory<T> : IClassFactory where T : IWidgetProvider, new()
    {
        private static readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");

        public int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                ppvObject = IntPtr.Zero;
                return unchecked((int)0x80040110);
            }

            if (riid == typeof(T).GUID || riid == IID_IUnknown)
            {
                ppvObject = MarshalInspectable<IWidgetProvider>.FromManaged(new T());
            }
            else
            {
                ppvObject = IntPtr.Zero;
                Environment.Exit(0);
            }

            return 0;
        }

        int IClassFactory.LockServer(bool fLock)
        {
            return 0;
        }
    }
}
