using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Content
{
    public sealed class ContentExternalBackdropLinkRcwFactoryAttribute : WinRTImplementationTypeRcwFactoryAttribute
    {
        public override object CreateInstance(IInspectable inspectable)
        {
            return new global::Microsoft.UI.Content.ContentExternalBackdropLink(inspectable.ObjRef);
        }
    }
}
