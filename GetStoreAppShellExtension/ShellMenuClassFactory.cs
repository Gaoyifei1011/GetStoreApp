using GetStoreAppShellExtension.Commands;
using GetStoreAppShellExtension.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension
{
    /// <summary>
    /// 允许创建对象的类
    /// </summary>
    [GeneratedComClass]
    public partial class ShellMenuClassFactory : IClassFactory
    {
        private readonly IExplorerCommand rootExplorerCommand = new RootExplorerCommand();

        public int CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject)
        {
            if (pUnkOuter != nint.Zero)
            {
                ppvObject = nint.Zero;
                return unchecked((int)0x80040110);
            }

            ppvObject = Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(rootExplorerCommand, CreateComInterfaceFlags.None);
            return 0;
        }

        public int LockServer(bool fLock)
        {
            return 0;
        }
    }
}
