using System;

namespace GetStoreApp.Extensions.Messaging
{
    /// <summary>
    /// 存储一个 <see cref="Action" /> ,而不会导致对Action的所有者创建硬引用。所有者可以在任何时候被垃圾收集。
    /// </summary>
    public class WeakAction
    {
        private readonly Action _action;

        private WeakReference _reference;

        /// <summary>
        /// 初始化<see cref="WeakAction" />类的一个新实例。
        /// </summary>
        /// <param name="target">Action的所有者。</param>
        /// <param name="action">将与此实例关联的操作。</param>
        public WeakAction(object target, Action action)
        {
            _reference = new WeakReference(target);
            _action = action;
        }

        /// <summary>
        /// 获取与此实例关联的操作。
        /// </summary>
        public Action Action
        {
            get
            {
                return _action;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示动作的所有者是否仍然存活，或者它是否已经被垃圾回收器收集。
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (_reference is null)
                {
                    return false;
                }

                return _reference.IsAlive;
            }
        }

        /// <summary>
        /// 获取动作的所有者。该对象存储为<see cref="WeakReference" />。
        /// </summary>
        public object Target
        {
            get
            {
                if (_reference is null)
                {
                    return null;
                }

                return _reference.Target;
            }
        }

        /// <summary>
        /// 执行操作。只有当动作的所有者仍然活着时才会发生这种情况。
        /// </summary>
        public void Execute()
        {
            if (_action is not null
                && IsAlive)
            {
                _action();
            }
        }

        /// <summary>
        /// 将此实例存储的引用设置为空。
        /// </summary>
        public void MarkForDeletion()
        {
            _reference = null;
        }
    }
}
