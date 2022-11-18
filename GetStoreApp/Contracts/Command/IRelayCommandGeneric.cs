#nullable enable

namespace GetStoreApp.Contracts.Command
{
    /// <summary>
    /// 表示更具体版本的通用接口 <see cref="IRelayCommand"/>。
    /// </summary>
    /// <typeparam name="T">用作接口方法参数的类型。</typeparam>
    public interface IRelayCommand<in T> : IRelayCommand
    {
        /// <summary>
        /// 提供 <see cref="IRelayCommand"/> 的强类型变体。
        /// </summary>
        /// <param name="parameter">输入参数。</param>
        /// <returns>是否可以执行当前命令。</returns>
        /// <remarks>使用此重载以避免装箱，如果 <typeparamref name="T"/> 是值类型。</remarks>
        bool CanExecute(T? parameter);

        /// <summary>
        /// 提供 <see cref="IRelayCommand"/> 的强类型变体。
        /// </summary>
        /// <param name="parameter">输入参数。</param>
        /// <remarks>使用此重载以避免装箱，如果 <typeparamref name="T"/> 是值类型。</remarks>
        void Execute(T? parameter);
    }
}
