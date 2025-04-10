using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("70CE8275-8D05-5222-A00B-41F9F9B2150C")]
    public partial interface IContentIslandPartner : IInspectable
    {
        [PreserveSig]
        int GetIslandInputSite(out IntPtr islandInputSitePartner);

        [PreserveSig]
        int Get_TEMP_DesktopSiteBridge(out IntPtr desktopSiteBridge);
    }
}
