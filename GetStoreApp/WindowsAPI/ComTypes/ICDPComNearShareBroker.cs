using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("6B8007AE-4DD7-46F3-9BEC-06F777D78864")]
    public partial interface ICDPComNearShareBroker
    {
        [PreserveSig]
        int CreateNearShareHandler(in Guid a, [MarshalAs(UnmanagedType.LPWStr)] string b, out nint ppv);

        [PreserveSig]
        int CreateNearShareSender(out ICDPComNearShareSender nearShareSender);
    }
}
