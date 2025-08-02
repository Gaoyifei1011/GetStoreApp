using Microsoft.UI.Xaml;

namespace GetStoreApp.Extensions.Behaviors
{
    /// <summary>
    /// 行为抽象接口
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// 获取附加到 IBehavior 的 DependencyObject
        /// </summary>
        DependencyObject AssociatedObject { get; }

        /// <summary>
        /// 附加到指定的对象
        /// </summary>
        /// <param name="associatedObject">附加到 IBehavior 的 DependencyObject</param>
        void Attach(DependencyObject associatedObject);

        /// <summary>
        /// 将此实例与其关联对象分离
        /// </summary>
        void Detach();
    }
}
