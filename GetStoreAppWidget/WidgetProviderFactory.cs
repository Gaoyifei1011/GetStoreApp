using GetStoreAppWidget.WindowsAPI.ComTypes;
using Microsoft.Windows.Widgets.Providers;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreAppWidget
{
    [GeneratedComClass]
    public partial class WidgetProviderFactory<T> : IClassFactory where T : IWidgetProvider, new()
    {
        public int CreateInstance(IntPtr pUnkOuter, in Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            if (pUnkOuter != IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(2147221232);
            }

            if (riid == typeof(T).GUID || riid == Guid.Empty)
            {
                ppvObject = MarshalInspectable<IWidgetProvider>.FromManaged(new T());
            }
            else
            {
                Marshal.ThrowExceptionForHR(2147467262);
            }

            return 0;
        }

        int IClassFactory.LockServer(bool fLock)
        {
            return 0;
        }
    }
}
