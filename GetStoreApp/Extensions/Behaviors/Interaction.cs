using Microsoft.UI.Xaml;

namespace GetStoreApp.Extensions.Behaviors
{
    /// <summary>
    /// 行为交互类
    /// </summary>
    public sealed class Interaction
    {
        /// <summary>
        /// 获取或设置与指定对象关联的行为集合
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, OnBehaviorsChanged));

        /// <summary>
        /// 获取与指定对象关联的行为集合
        /// </summary>
        /// <param name="dependencyObject">从行为集合中检索依赖对象</param>
        /// <returns>包含与指定对象相关联的行为的行为集合</returns>
        public static BehaviorCollection GetBehaviors(DependencyObject dependencyObject)
        {
            if (dependencyObject is not null)
            {
                BehaviorCollection behaviorCollection = (BehaviorCollection)dependencyObject.GetValue(BehaviorsProperty);
                if (behaviorCollection is null)
                {
                    behaviorCollection = [];
                    dependencyObject.SetValue(BehaviorsProperty, behaviorCollection);
                    if (dependencyObject is FrameworkElement frameworkElement)
                    {
                        frameworkElement.Loaded -= OnLoaded;
                        frameworkElement.Loaded += OnLoaded;
                        frameworkElement.Unloaded -= OnUnloaded;
                        frameworkElement.Unloaded += OnUnloaded;
                    }
                }

                return behaviorCollection;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置与指定对象关联的行为集合
        /// </summary>
        /// <param name="obj">设置与指定对象关联的行为集合</param>
        /// <param name="value">与对象关联的行为集合</param>
        public static void SetBehaviors(DependencyObject dependencyObject, BehaviorCollection value)
        {
            dependencyObject?.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        /// 行为发生改变时触发的事件
        /// </summary>
        private static void OnBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BehaviorCollection behaviorCollection = (BehaviorCollection)args.OldValue;
            BehaviorCollection behaviorCollection2 = (BehaviorCollection)args.NewValue;
            if (!Equals(behaviorCollection, behaviorCollection2))
            {
                if (behaviorCollection is not null && behaviorCollection.AssociatedObject is not null)
                {
                    behaviorCollection.Detach();
                }

                if (behaviorCollection2 is not null && sender is not null)
                {
                    behaviorCollection2.Attach(sender);
                }
            }
        }

        /// <summary>
        /// 控件加载完成后触发的事件
        /// </summary>
        private static void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is DependencyObject dependencyObject)
            {
                GetBehaviors(dependencyObject).Attach(dependencyObject);
            }
        }

        /// <summary>
        /// 控件卸载完成后触发的事件
        /// </summary>
        private static void OnUnloaded(object sender, RoutedEventArgs args)
        {
            if (sender is DependencyObject dependencyObject)
            {
                GetBehaviors(dependencyObject).Detach();
            }
        }
    }
}
