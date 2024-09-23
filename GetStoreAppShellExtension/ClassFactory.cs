using GetStoreAppShellExtension.Commands;
using GetStoreAppShellExtension.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComClass]
    public partial class ClassFactory : IClassFactory
    {
        private readonly StrategyBasedComWrappers strategyBasedComWrappers = new();
        private readonly Func<object> rootExplorerCommandFunc = new(() => { return new RootExplorerCommand(); });

        public int CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject)
        {
            if (pUnkOuter != nint.Zero)
            {
                ppvObject = nint.Zero;
                return unchecked((int)0x80040110); // CLASS_E_NOAGGREGATION
            }

            object obj = rootExplorerCommandFunc.Invoke();
            ppvObject = strategyBasedComWrappers.GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None);
            return 0;
        }

        public int LockServer(bool fLock)
        {
            if (fLock)
            {
                Program.DllAddRef();
            }
            else
            {
                Program.DllRelease();
            }

            return 0;
        }
    }
}
