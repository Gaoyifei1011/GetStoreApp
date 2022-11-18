using GetStoreApp.Contracts.Command;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Input;

#nullable enable

namespace GetStoreApp.Extensions.Command
{
    /// <summary>
    ///  一个通用命令，其唯一用途是通过调用委托将其功能中继到其他对象。CanExecute 方法的默认返回值是 <see langword="true"/>。
    ///  此类允许您接受 <see cref="Execute(T)"/> 和 <see cref ="CanExecute(T)"/> 回调方法中的命令参数。 </summary>
    /// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
    public sealed class RelayCommand<T> : IRelayCommand<T>
    {
        /// <summary>
        /// <see cref="Execute(T)"/> 使用时需要调用 <see cref="Action"/>
        /// </summary>
        private readonly Action<T?> execute;

        /// <summary>
        /// 使用 <see cref="CanExecute(T)"/> 时要调用的可选操作。
        /// </summary>
        private readonly Predicate<T?>? canExecute;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 初始化始终可以执行的 <see cref="RelayCommand{T}"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行逻辑。</param>
        /// <remarks>
        /// 由于 <see cref="ICommand" /> 接口公开了接受可为空<see cref="object"/> 参数的方法，因此建议如果 <typeparamref name="T"/> 是引用类型，
        /// 则应始终将其声明为可为空，并始终在 <paramref name="execute"/> 中执行检查。
        /// </remarks>
        /// 当 <paramref name="execute"/> 为 <see langword="null"/> 时，抛出 <exception cref="ArgumentNullException"> 异常
        public RelayCommand(Action<T?> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            this.execute = execute;
        }

        /// <summary>
        /// 初始化可以执行的 <see cref="RelayCommand{T}"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行逻辑。</param>
        /// <param name="canExecute">执行状态逻辑。</param>
        /// <remarks>查看 <see cref="RelayCommand{T}(Action{T})"/> 的标注。</remarks>
        /// 当 <paramref name="execute"/> 或 <paramref name="canExecute"/> 为 <see langword="null"/> 时，抛出 <exception cref="ArgumentNullException"> 异常
        public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(T? parameter)
        {
            return this.canExecute?.Invoke(parameter) != false;
        }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            // 特殊情况，值类型参数类型的空值。
            // 这可确保在初始化期间不会引发任何异常。
            if (parameter is null && default(T) is not null)
            {
                return false;
            }

            if (!TryGetCommandArgument(parameter, out T? result))
            {
                ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            return CanExecute(result);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(T? parameter)
        {
            this.execute(parameter);
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            if (!TryGetCommandArgument(parameter, out T? result))
            {
                ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            Execute(result);
        }

        /// <summary>
        /// 尝试从输入中获取兼容类型 <typeparamref name="T"/> 的命令参数 <see cref=“object”/>。
        /// </summary>
        /// <param name="parameter">输入参数。</param>
        /// <param name="result">生成的 <typeparamref name="T"/> 值（如果有）。</param>
        /// <returns>是否可以检索兼容的命令参数。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryGetCommandArgument(object? parameter, out T? result)
        {
            // 如果参数为 null，并且 T 的默认值也为 null，则该参数有效。T 可以是引用类型或可为 null 的值类型。
            if (parameter is null && default(T) is null)
            {
                result = default;

                return true;
            }

            // 检查参数是否为 T 值，因此 T 的类型实例或派生类型是引用类型，如果 T 是接口，则为接口实现，
            // 如果 T 是值类型，则为装箱值类型。
            if (parameter is T argument)
            {
                result = argument;

                return true;
            }

            result = default;

            return false;
        }

        /// <summary>
        /// 如果使用了无效的命令参数，抛出 <see cref="ArgumentException"/> 异常。
        /// </summary>
        /// <param name="parameter">输入参数。</param>
        /// <exception cref="ArgumentException">抛出错误消息，以提供有关无效参数的信息。</exception>
        [DoesNotReturn]
        internal static void ThrowArgumentExceptionForInvalidCommandArgument(object? parameter)
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static Exception GetException(object? parameter)
            {
                if (parameter is null)
                {
                    return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) must not be null, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
                }

                return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) cannot be of type {parameter.GetType()}, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
            }

            throw GetException(parameter);
        }
    }
}
