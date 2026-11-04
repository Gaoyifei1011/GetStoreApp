using Microsoft.UI.Xaml.CustomAttributes;
using Windows.Foundation.Metadata;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    [WindowsRuntimeType("Microsoft.UI.Xaml")]
    [global::System.Runtime.InteropServices.Guid("2212A81F-DAC5-565C-BBEA-5A5FB4F82F68")]
    [WindowsRuntimeHelperType(typeof(ABI.Microsoft.UI.Xaml.Controls.IWrapPanel))]
    [MUXPropertyChangedCallback(enable = true)]
    [MUXPropertyChangedCallbackMethodName(value = "OnPropertyChanged")]
    [ContractVersion(typeof(XamlContract), 720896u)]
    internal interface IWrapPanel
    {
        double ItemSpacing { get; set; }

        WrapPanelItemsStretch ItemsStretch { get; set; }

        double LineSpacing { get; set; }

        Orientation Orientation { get; set; }

        Thickness Padding { get; set; }
    }
}