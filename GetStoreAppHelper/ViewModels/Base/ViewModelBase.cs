using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreAppHelper.ViewModels.Base
{
    /// <summary>
    /// 该视图模型基类实现了InotifyPropertyChanged接口用于通知UI更新
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
