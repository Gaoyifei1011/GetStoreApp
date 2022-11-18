using System.Windows.Input;

namespace GetStoreApp.Contracts.Command
{
    /// <summary>
    /// 一个扩展的接口 <see cref="ICommand"/> ，能够从外部引发 <see cref="ICommand.CanExecuteChanged"/> 事件。
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// 通知 <see cref="ICommand.CanExecute"/> 属性已更改。
        /// </summary>
        void NotifyCanExecuteChanged();
    }
}
