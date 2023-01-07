using GetStoreApp.Contracts.Messaging;
using System;

namespace GetStoreApp.Extensions.Messaging
{
    /// <summary>
    /// 存储一个Action，但不会导致创建一个指向Action所有者的硬引用。所有者可以在任何时候被垃圾收集。
    /// </summary>
    /// <typeparam name="T">Action参数的类型。</typeparam>
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private readonly Action<T> _action;

        /// <summary>
        /// 初始化WeakAction类的一个新实例。
        /// </summary>
        /// <param name="target">Action的所有者。</param>
        /// <param name="action">将与此实例关联的操作。</param>
        public WeakAction(object target, Action<T> action)
            : base(target, null)
        {
            _action = action;
        }

        /// <summary>
        /// 获取与此实例关联的操作。
        /// </summary>
        public new Action<T> Action
        {
            get
            {
                return _action;
            }
        }

        /// <summary>
        /// 执行操作。只有当动作的所有者仍然活着时才会发生这种情况。动作的参数被设置为default(T)。
        /// </summary>
        public new void Execute()
        {
            if (_action is not null
                && IsAlive)
            {
                _action(default(T));
            }
        }

        /// <summary>
        /// 执行操作。只有当动作的所有者仍然活着时才会发生这种情况。
        /// </summary>
        /// <param name="parameter">要传递给操作的参数。</param>
        public void Execute(T parameter)
        {
            if (_action is not null
                && IsAlive)
            {
                _action(parameter);
            }
        }

        /// <summary>
        /// 使用object类型的参数执行操作。此参数将转换为T。此方法实现了<see cref="IExecuteWithObject" />。如果你存储了多个WeakAction{T}实例，但事先不知道类型T代表什么，这个方法很有用。
        /// </summary>
        /// <param name="parameter">转换为T后将传递给操作的参数。</param>
        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }
    }
}
