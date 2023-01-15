using System;

namespace GetStoreApp.Contracts.Messaging
{
    /// <summary>
    /// 信使是一个允许对象交换消息的类。
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// 为某种类型的消息注册收信人。发送相应消息时将执行操作参数。
        /// <para>注册收信人不会创建对它的硬引用，因此如果删除此收信人，则不会导致内存泄漏。</para>
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="recipient">将接收消息的收信人。</param>
        /// <param name="action">发送类型为 TMessage 的消息时将执行的操作。</param>
        void Register<TMessage>(object recipient, Action<TMessage> action);

        /// <summary>
        /// 为某种类型的消息注册收信人。发送相应消息时将执行操作参数。有关如何接收从 TMessage 派生的消息（或者，如果 TMessage 是接口，则实现 TMessage 的消息）的详细信息，请参阅 receiveDerivedMessagesToo 参数。
        /// <para>注册收信人不会创建对它的硬引用，因此如果删除此收信人，则不会导致内存泄漏。</para>
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
        void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action);

        /// <summary>
        /// 向已注册的收信人发送消息。消息将到达使用 Register 方法之一注册此消息类型的所有收信人。
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <param name="message">要发送给已注册收信人的消息。</param>
        void Send<TMessage>(TMessage message);

        /// <summary>
        /// 向已注册的收信人发送消息。消息将仅到达使用 Register 方法之一注册此消息类型且属于 targetType 的收信人。
        /// </summary>
        /// <typeparam name="TMessage">收信人注册的消息类型。</typeparam>
        /// <typeparam name="TTarget">将接收消息的收信人的类型。消息不会发送给其他类型的收信人。</typeparam>
        /// <param name="message">要发送给已注册收信人的消息。</param>
        void Send<TMessage, TTarget>(TMessage message);

        /// <summary>
        /// 完全注销消息收信人。执行此方法后，收信人将不再收到任何消息。
        /// </summary>
        /// <param name="recipient">必须取消注册的收信人。</param>
        void Unregister(object recipient);

        /// <summary>
        /// 仅为给定类型的消息取消注册消息收信人。执行此方法后，收信人将不再接收 TMessage 类型的消息，但仍会收到其他消息类型（如果它之前已注册）。
        /// </summary>
        /// <typeparam name="TMessage">收信人要从中注销的消息类型。</typeparam>
        /// <param name="recipient">必须取消注册的收信人。</param>
        void Unregister<TMessage>(object recipient);

        /// <summary>
        /// 为给定类型的消息和给定操作取消注册消息收信人。其他消息类型仍将传输给收信人（如果收信人之前已注册）。
        /// 已为消息类型 TMessage 和给定收信人（如果可用）注册的其他操作也将保持可用。
        /// </summary>
        /// <typeparam name="TMessage">收信人要从中注销的消息类型。</typeparam>
        /// <param name="recipient">必须取消注册的收信人。</param>
        /// <param name="action">必须为收信人和消息类型 TMessage 取消注册的操作。</param>
        void Unregister<TMessage>(object recipient, Action<TMessage> action);
    }
}
