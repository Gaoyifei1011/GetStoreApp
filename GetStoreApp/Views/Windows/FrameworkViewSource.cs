using Windows.ApplicationModel.Core;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 创建视图，特别是 FrameworkView 实例
    /// </summary>
    public sealed class FrameworkViewSource : IFrameworkViewSource
    {
        /// <summary>
        /// 创建 FrameworkView
        /// </summary>
        public IFrameworkView CreateView()
        {
            return new FrameworkView();
        }
    }
}
