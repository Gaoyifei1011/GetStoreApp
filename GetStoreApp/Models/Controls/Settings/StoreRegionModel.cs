using System.ComponentModel;
using Windows.Globalization;

namespace GetStoreApp.Models.Controls.Settings
{
    /// <summary>
    /// 区域模型
    /// </summary>
    public sealed partial class StoreRegionModel : INotifyPropertyChanged
    {
        public GeographicRegion StoreRegionInfo { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
