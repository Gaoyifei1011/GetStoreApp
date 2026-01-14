using Microsoft.UI.Composition;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

// 抑制 IDE1006 警告
#pragma warning disable IDE1006

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("1054BF83-B35B-5FDE-8DD7-AC3BB3E6CE27")]
    public partial interface IContentExternalBackdropLink : IInspectable
    {
        [PreserveSig]
        int get_DispatcherQueue(out nint dispatcherQueue);

        [PreserveSig]
        int get_ExternalBackdropBorderMode(out CompositionBorderMode compositionBorderMode);

        [PreserveSig]
        int set_ExternalBackdropBorderMode(CompositionBorderMode compositionBorderMode);

        [PreserveSig]
        int get_PlacementVisual(out nint placementVisual);
    }
}
