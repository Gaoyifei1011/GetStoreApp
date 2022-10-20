using GetStoreApp.Extensions.Enum;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.Contracts.Extensions
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    public interface IShellItem
    {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object BindToHandler(
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        IShellItem GetParent();

        void GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        SFGAOF GetAttributes(SFGAOF sfgaoMask);

        int Compare(IShellItem psi, SICHINTF hint);
    }
}
