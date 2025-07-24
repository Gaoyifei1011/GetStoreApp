using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("6090202D-2843-4BA5-9B0D-FC88EECD9CE5")]
    public partial interface ICoreApplicationPrivate2 : IInspectable
    {
        [PreserveSig]
        int InitializeForAttach();

        [PreserveSig]
        int WaitForActivate(out nint coreWindow);

        [PreserveSig]
        int CreateNonImmersiveView(out nint coreApplicationView);
    }
}
