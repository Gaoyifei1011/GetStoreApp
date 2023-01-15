namespace GetStoreApp.Contracts.Messaging
{
    /// <summary>
    /// 此接口适用于 <see cref="WeakAction{T}" /> 类，如果您存储多个 WeakAction{T} 实例但事先不知道 T 代表什么类型，则此接口非常有用。
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="parameter">作为要强制转换为相应类型的对象传递的参数。</param>
        void ExecuteWithObject(object parameter);
    }
}
