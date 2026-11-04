using Microsoft.UI.Xaml.CustomAttributes;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Metadata;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    [WindowsRuntimeType("Microsoft.UI.Xaml")]
    [global::System.Runtime.InteropServices.Guid("46CF8953-F9EB-5C6E-A976-1B8E43B0609E")]
    [WindowsRuntimeHelperType(typeof(ABI.Microsoft.UI.Xaml.Controls.ILayoutPanel))]
    [MUXPropertyChangedCallback(enable = true)]
    [MUXPropertyChangedCallbackMethodName(value = "OnPropertyChanged")]
    [ContractVersion(typeof(XamlContract), 720896u)]
    internal interface ILayoutPanel
    {
        Brush BorderBrush { get; set; }

        Thickness BorderThickness { get; set; }

        CornerRadius CornerRadius { get; set; }

        Layout Layout { get; set; }

        Thickness Padding { get; set; }
    }
}
