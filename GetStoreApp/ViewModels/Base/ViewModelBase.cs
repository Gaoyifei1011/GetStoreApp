using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.ViewModels.Base
{
    /// <summary>
    /// 该ViewModelModelBase类实现了InotifyPropertyChanged接口用于通知UI更新
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
