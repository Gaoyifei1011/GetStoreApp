using System;

namespace GetStoreApp.Extensions.Messaging
{
    /// <summary>
    /// Stores an <see cref="Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    ////[ClassInfo(typeof(Messenger))]
    public class WeakAction
    {
        private readonly Action _action;

        private WeakReference _reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            _reference = new WeakReference(target);
            _action = action;
        }

        /// <summary>
        /// Gets the Action associated to this instance.
        /// </summary>
        public Action Action
        {
            get
            {
                return _action;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (_reference == null)
                {
                    return false;
                }

                return _reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (_reference == null)
                {
                    return null;
                }

                return _reference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (_action != null
                && IsAlive)
            {
                _action();
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            _reference = null;
        }
    }
}
