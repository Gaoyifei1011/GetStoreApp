using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Helpers
{
    public static class FrameExtensionsHelper
    {
        public static object GetPageViewModel(this Frame frame)
            => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
    }
}
