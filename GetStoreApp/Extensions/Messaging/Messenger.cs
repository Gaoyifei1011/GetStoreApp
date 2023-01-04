using GetStoreApp.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreApp.Extensions.Messaging
{
    /// <summary>
    /// Messenger 是一个允许对象交换消息的类。
    /// </summary>
    public class Messenger : IMessenger
    {
        private static Messenger _defaultInstance;

        private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;

        private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;

        /// <summary>
        /// 获取 Messenger's 的默认实例，允许以静态方式注册和发送消息。
        /// </summary>
        public static Messenger Default
        {
            get
            {
                _defaultInstance ??= new Messenger();

                return _defaultInstance;
            }
        }

        /// <summary>
        /// 提供一种使用自定义实例覆盖 Messenger.Default 实例的方法，例如用于单元测试目的。
        /// </summary>
        /// <param name="newMessenger">将用作 Messenger.Default 的实例。</param>
        public static void OverrideDefault(Messenger newMessenger)
        {
            _defaultInstance = newMessenger;
        }

        /// <summary>
        /// 将信使的默认（静态）实例设置为 null。
        /// </summary>
        public static void Reset()
        {
            _defaultInstance = null;
        }

        /// <summary>
        /// 为某种类型的消息注册收信人。发送相应消息时将执行操作参数。
        /// <para>注册收信人不会创建对它的硬引用，因此如果删除此收信人，则不会导致内存泄漏。</para>
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="recipient">消息的收信人。</param>
        /// <param name="action">发送类型为 TMessage 的消息时将执行的操作。</param>
        public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Register(recipient, null, false, action);
        }

        /// <summary>
        /// 为某种类型的消息注册收信人。发送相应消息时将执行操作参数。有关如何接收从 TMessage 派生的消息（或者，如果 TMessage 是接口，则实现 TMessage 的消息）的详细信息，请参阅 receiveDerivedMessagesToo 参数。
        /// <para>注册收信人不会创建对它的硬引用，因此如果删除此收信人，则不会导致内存泄漏。
        /// </para>
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="recipient">准备接收消息的收信人。</param>
        /// <param name="receiveDerivedMessagesToo">
        /// 如果为 true，则派生自 TMessage 的消息类型也将传输给接收方。例如，如果 SendOrderMessage 和 ExecuteOrderMessage 派生自 OrderMessage，
        /// 则注册 OrderMessage 并将 receiveDerivedMessagesToo 设置为 true 会将 SendOrderMessage 和 ExecuteOrderMessage 发送给注册的收信人。
        /// <para>
        /// 此外，如果 TMessage 是一个接口，则实现 TMessage 的消息类型也将传输给接收方。例如，如果 SendOrderMessage 和 ExecuteOrderMessage 实现了 IOrderMessage，
        /// 则注册 IOrderMessage 并将 receiveDerivedMessagesToo 设置为 true 会将 SendOrderMessage 和 ExecuteOrderMessage 发送给注册的收信人。
        /// </para>
        /// </param>
        /// <param name="action">发送类型为 TMessage 的消息时将执行的操作。</param>
        public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            Register(recipient, null, receiveDerivedMessagesToo, action);
        }

        /// <summary>
        /// 为某种类型的消息注册收信人。发送相应消息时将执行操作参数。
        /// <para>注册收信人不会创建对它的硬引用，因此如果删除此收信人，则不会导致内存泄漏。</para>
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="recipient">准备接收消息的收信人。</param>
        /// <param name="token">
        /// 消息传递通道的令牌。如果收信人使用令牌注册，并且发件人使用相同的令牌发送消息，则此消息将传递给收信人。注册时未使用令牌（或使用其他令牌）
        /// 的其他收信人将不会收到该消息。同样，在没有任何令牌或具有不同令牌的情况下发送的消息将不会传递给该收信人。
        /// </param>
        /// <param name="action">发送类型为 TMessage 的消息时将执行的操作。 </param>
        public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Register(recipient, token, false, action);
        }

        /// <summary>
        /// 为某种类型的消息注册收件人。发送相应消息时将执行操作参数。请参阅 receiveDerivedMessagesToo 参数，了解有关如何接收从 TMessage 派生的消息（或者，如果 TMessage 是一个接口，则实现 TMessage 的消息）的详细信息。
        /// <para>注册收件人不会创建对它的硬引用，因此如果删除此收件人，则不会导致内存泄漏。</para>
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="recipient">准备接收消息的收信人。</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        /// <param name="receiveDerivedMessagesToo">
        /// 如果为 true，则派生自 TMessage 的消息类型也将传输给接收方。例如，如果 SendOrderMessage 和 ExecuteOrderMessage 派生自 OrderMessage，
        /// 则注册 OrderMessage 并将 receiveDerivedMessagesToo 设置为 true 会将 SendOrderMessage 和 ExecuteOrderMessage 发送给注册的收件人。
        /// <para>
        /// 此外，如果 TMessage 是一个接口，则实现 TMessage 的消息类型也将传输给接收方。例如，如果 SendOrderMessage 和 ExecuteOrderMessage 实现了 IOrderMessage，则注册 IOrderMessage 并将 receiveDerivedMessagesToo 设置为 true 会将 SendOrderMessage 和 ExecuteOrderMessage 发送给注册的收件人。
        /// </para>
        /// </param>
        /// <param name="action">发送类型为 TMessage 的消息时将执行的操作。 </param>
        public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            var messageType = typeof(TMessage);

            Dictionary<Type, List<WeakActionAndToken>> recipients;

            if (receiveDerivedMessagesToo)
            {
                _recipientsOfSubclassesAction ??= new Dictionary<Type, List<WeakActionAndToken>>();

                recipients = _recipientsOfSubclassesAction;
            }
            else
            {
                _recipientsStrictAction ??= new Dictionary<Type, List<WeakActionAndToken>>();

                recipients = _recipientsStrictAction;
            }

            List<WeakActionAndToken> list;

            if (!recipients.ContainsKey(messageType))
            {
                list = new List<WeakActionAndToken>();
                recipients.Add(messageType, list);
            }
            else
            {
                list = recipients[messageType];
            }

            var weakAction = new WeakAction<TMessage>(recipient, action);
            var item = new WeakActionAndToken
            {
                Action = weakAction,
                Token = token
            };
            list.Add(item);

            Cleanup();
        }

        /// <summary>
        /// 向已注册的收件人发送邮件。邮件将到达使用 Register 方法之一注册此邮件类型的所有收件人。
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="message">要发送给已注册收件人的邮件。</param>
        public virtual void Send<TMessage>(TMessage message)
        {
            SendToTargetOrType(message, null, null);
        }

        /// <summary>
        /// 向已注册的收件人发送邮件。邮件将仅到达使用 Register 方法之一注册此邮件类型且属于 targetType 的收件人。
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <typeparam name="TTarget">将接收邮件的收件人的类型。邮件不会发送给其他类型的收件人。</typeparam>
        /// <param name="message">要发送给已注册收件人的邮件。</param>
        public virtual void Send<TMessage, TTarget>(TMessage message)
        {
            SendToTargetOrType(message, typeof(TTarget), null);
        }

        /// <summary>
        /// 向已注册的收件人发送邮件。邮件将仅到达使用 Register 方法之一注册此邮件类型且属于 targetType 的收件人。
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="message">要发送给已注册收件人的邮件。</param>
        /// <param name="token">
        /// 消息传递通道的令牌。如果收件人使用令牌注册，并且发件人使用相同的令牌发送邮件，则此邮件将传递给收件人。
        /// 注册时未使用令牌（或使用其他令牌）的其他收件人将不会收到该消息。同样，在没有任何令牌或具有不同令牌的情况下发送的邮件将不会传递给该收件人。
        /// </param>
        public virtual void Send<TMessage>(TMessage message, object token)
        {
            SendToTargetOrType(message, null, token);
        }

        /// <summary>
        /// 完全注销邮件收件人。执行此方法后，收件人将不再收到任何消息。
        /// </summary>
        /// <param name="recipient">必须取消注册的收件人。</param>
        public virtual void Unregister(object recipient)
        {
            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
        }

        /// <summary>
        /// 仅为给定类型的邮件取消注册邮件收件人。执行此方法后，收件人将不再接收 TMessage 类型的消息，但仍会收到其他消息类型（如果它之前已注册）。
        /// </summary>
        /// <param name="recipient">必须取消注册的收件人。</param>
        /// <typeparam name="TMessage">收件人要从中取消注册的邮件类型。</typeparam>
        public virtual void Unregister<TMessage>(object recipient)
        {
            Unregister<TMessage>(recipient, null);
        }

        /// <summary>
        /// 仅为给定类型的消息和给定令牌注销消息收件人。执行此方法后，接收方将不再接收具有给定令牌的 TMessage 类型的消息，
        /// 但仍将接收其他消息类型或具有其他令牌的消息（如果它以前注册了这些消息）。
        /// </summary>
        /// <param name="recipient">必须取消注册的收件人。</param>
        /// <param name="token">必须取消注册收件人的令牌。</param>
        /// <typeparam name="TMessage">收件人要从中注销的邮件类型。</typeparam>
        public virtual void Unregister<TMessage>(object recipient, object token)
        {
            Unregister<TMessage>(recipient, token, null);
        }

        /// <summary>
        /// 为给定类型的邮件和给定操作取消注册邮件收件人。其他邮件类型仍将传输给收件人（如果收件人之前已注册）。
        /// 已为消息类型 TMessage 和给定收件人（如果可用）注册的其他操作也将保持可用。
        /// </summary>
        /// <typeparam name="TMessage">收件人要从中注销的邮件类型。</typeparam>
        /// <param name="recipient">必须取消注册的收件人。</param>
        /// <param name="action">必须为收件人和消息类型 TMessage 取消注册的操作。</param>
        public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            UnregisterFromLists(recipient, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, action, _recipientsOfSubclassesAction);
            Cleanup();
        }

        /// <summary>
        /// 为给定类型的消息、给定操作和给定令牌注销消息收件人。其他邮件类型仍将传输给收件人（如果收件人之前已注册）。为消息类型 TMessage 注册的其他操作、给定收件人和其他令牌（如果可用）也将保持可用。
        /// </summary>
        /// <typeparam name="TMessage">收件人要从中注销的邮件类型。</typeparam>
        /// <param name="recipient">必须取消注册的收件人。</param>
        /// <param name="token">必须取消注册收件人的令牌。</param>
        /// <param name="action">必须为收件人和消息类型 TMessage 取消注册的操作。</param>
        public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
            Cleanup();
        }

        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists is null)
            {
                return;
            }

            var listsToRemove = new List<Type>();
            foreach (var list in lists)
            {
                var recipientsToRemove = new List<WeakActionAndToken>();
                foreach (var item in list.Value)
                {
                    if (item.Action is null
                        || !item.Action.IsAlive)
                    {
                        recipientsToRemove.Add(item);
                    }
                }

                foreach (var recipient in recipientsToRemove)
                {
                    list.Value.Remove(recipient);
                }

                if (list.Value.Count is 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (var key in listsToRemove)
            {
                lists.Remove(key);
            }
        }

        private static bool Implements(Type instanceType, Type interfaceType)
        {
            if (interfaceType is null
                || instanceType is null)
            {
                return false;
            }

            Type[] interfaces = instanceType.GetInterfaces();
            foreach (var currentInterface in interfaces)
            {
                if (currentInterface == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SendToList<TMessage>(
            TMessage message,
            IEnumerable<WeakActionAndToken> list,
            Type messageTargetType,
            object token)
        {
            if (list != null)
            {
                // Clone to protect from people registering in a "receive message" method
                // Bug correction Messaging BL0004.007
                var listClone = list.Take(list.Count()).ToList();

                foreach (var item in listClone)
                {
                    if (item.Action is IExecuteWithObject executeAction
                        && item.Action.IsAlive
                        && item.Action.Target != null
                        && (messageTargetType is null
                            || item.Action.Target.GetType() == messageTargetType
                            || Implements(item.Action.Target.GetType(), messageTargetType))
                        && ((item.Token is null && token is null)
                            || item.Token != null && item.Token.Equals(token)))
                    {
                        executeAction.ExecuteWithObject(message);
                    }
                }
            }
        }

        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient is null
                || lists is null
                || lists.Count is 0)
            {
                return;
            }

            lock (lists)
            {
                foreach (var messageType in lists.Keys)
                {
                    foreach (var item in lists[messageType])
                    {
                        var weakAction = item.Action;

                        if (weakAction != null
                            && recipient == weakAction.Target)
                        {
                            weakAction.MarkForDeletion();
                        }
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(
            object recipient,
            Action<TMessage> action,
            Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            var messageType = typeof(TMessage);

            if (recipient is null
                || lists is null
                || lists.Count is 0
                || !lists.ContainsKey(messageType))
            {
                return;
            }

            lock (lists)
            {
                foreach (var item in lists[messageType])
                {
                    if (item.Action is WeakAction<TMessage> weakActionCasted
                        && recipient == weakActionCasted.Target
                        && (action is null
                            || action == weakActionCasted.Action))
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(
            object recipient,
            object token,
            Action<TMessage> action,
            Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            Type messageType = typeof(TMessage);

            if (recipient is null
                || lists is null
                || lists.Count is 0
                || !lists.ContainsKey(messageType))
            {
                return;
            }

            lock (lists)
            {
                foreach (WeakActionAndToken item in lists[messageType])
                {
                    if (item.Action is WeakAction<TMessage> weakActionCasted
                        && recipient == weakActionCasted.Target
                        && (action is null
                            || action == weakActionCasted.Action)
                        && (token is null
                            || token.Equals(item.Token)))
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }

        private void Cleanup()
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
        }

        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            var messageType = typeof(TMessage);

            if (_recipientsOfSubclassesAction != null)
            {
                // Clone to protect from people registering in a "receive message" method
                // Bug correction Messaging BL0008.002
                var listClone = _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList();

                foreach (var type in listClone)
                {
                    List<WeakActionAndToken> list = null;

                    if (messageType == type
                        || messageType.IsSubclassOf(type)
                        || Implements(messageType, type))
                    {
                        list = _recipientsOfSubclassesAction[type];
                    }

                    SendToList(message, list, messageTargetType, token);
                }
            }

            if (_recipientsStrictAction != null)
            {
                if (_recipientsStrictAction.TryGetValue(messageType, out List<WeakActionAndToken> value))
                {
                    var list = value;
                    SendToList(message, list, messageTargetType, token);
                }
            }

            Cleanup();
        }

        private struct WeakActionAndToken
        {
            public WeakAction Action;

            public object Token;
        }
    }
}
