using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// WinGet 程序包页面数据模型
    /// </summary>
    public sealed class WinGetViewModel : ViewModelBase
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }
    }
}
