using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("46CAC6FB-BB51-510A-958D-E0EB4160F678")]
    public partial interface IContentExternalBackdropLinkStatics : IInspectable
    {
        [PreserveSig]
        int Create(nint compositor, [MarshalAs(UnmanagedType.Interface)] out IContentExternalBackdropLink contentExternalBackdropLink);
    }
}
