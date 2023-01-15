using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Models.Base
{
    /// <summary>
    /// 该模型基类实现了InotifyPropertyChanged接口，当属性值发生修改时，可以通知UI界面的内容进行更新
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
