using GetStoreAppHelper.Contracts.Command;
using System;
using System.Runtime.CompilerServices;

#nullable enable

namespace GetStoreAppHelper.Extensions.Command
{
    /// <summary>
    /// 一个命令，其唯一用途是通过调用委托将其功能中继到其他对象。
    /// <see cref="CanExecute" /> 方法的默认返回值是 <see langword="true" />。 此类型不允许接受 <see cref="Execute" /> 和 <see cref="CanExecute"/> 回调方法中的命令参数。
    /// </summary>
    public sealed class RelayCommand : IRelayCommand
    {
        /// <summary>
        /// <see cref="Execute"/> 使用时需要调用 <see cref="Action"/>
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// 使用 <see cref="CanExecute"/> 时要调用的可选操作。
        /// </summary>
        private readonly Func<bool>? canExecute;

        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生。
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 初始化始终可以执行的 <see cref="RelayCommand"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行逻辑。</param>
        /// 当 <paramref name="execute"/> 为 <see langword="null"/> 时，抛出 <exception cref="ArgumentNullException"> 异常
        public RelayCommand(Action execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));

            this.execute = execute;
        }

        /// <summary>
        /// 初始化可以执行的 <see cref="RelayCommand"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行逻辑。</param>
        /// <param name="canExecute">执行状态逻辑。</param>
        /// 当 <paramref name="execute"/> 或 <paramref name="canExecute"/> 为 <see langword="null"/> 时，抛出 <exception cref="ArgumentNullException"> 异常
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 定义确定此命令是否可在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。 如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        /// <returns>如果可执行此命令，则为 true；否则为 false。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() is not false;
        }

        /// <summary>
        /// 定义在调用此命令时要调用的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。 如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        public void Execute(object? parameter)
        {
            execute();
        }
    }
}
