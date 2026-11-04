using System.ComponentModel;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class LayoutPanelRcwFactoryAttribute : WinRTImplementationTypeRcwFactoryAttribute
    {
        public override object CreateInstance(IInspectable inspectable)
        {
            return new global::Microsoft.UI.Xaml.Controls.LayoutPanel(inspectable.ObjRef);
        }
    }
}
