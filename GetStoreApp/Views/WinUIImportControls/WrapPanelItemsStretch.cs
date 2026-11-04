using Windows.Foundation.Metadata;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    [WindowsRuntimeType("Microsoft.UI.Xaml")]
    [WinRTExposedType(typeof(EnumTypeDetails<WrapPanelItemsStretch>))]
    [ContractVersion(typeof(XamlContract), 720896u)]
    public enum WrapPanelItemsStretch
    {
        None,
        Last
    }
}