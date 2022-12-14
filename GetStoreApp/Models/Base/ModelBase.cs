using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Models.Base
{
    /// <summary>
    /// 该ModelBase类既实现了DependancyObject类用于依赖属性，也实现了InotifyPropertyChanged接口用于通知UI更新
    /// </summary>
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
