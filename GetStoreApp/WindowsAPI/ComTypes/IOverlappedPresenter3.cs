using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Graphics;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("FF7841F0-8253-5ADF-ABBA-57919BFE1CAC")]
    public partial interface IOverlappedPresenter3 : IInspectable
    {
        [PreserveSig]
        public int GetPreferredMaximumSize(out SizeInt32 size);

        [PreserveSig]
        public void SetPreferredMaximumSize(SizeInt32 size);

        [PreserveSig]
        public int GetPreferredMinimumSize(out SizeInt32 size);

        [PreserveSig]
        public void SetPreferredMinimumSize(SizeInt32 size);

        [PreserveSig]
        public void SetPreferredBounds(SizeInt32 preferredMinimumSize, SizeInt32 preferredMaximumSize);
    }
}
