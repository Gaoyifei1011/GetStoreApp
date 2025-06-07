using GetStoreApp.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用包存放卷对话框
    /// </summary>
    public sealed partial class PackageVolumeDialog : ContentDialog, INotifyPropertyChanged
    {
        private PackageVolumeModel _currentPackageVolume;

        public PackageVolumeModel CurrentPackageVolume
        {
            get { return _currentPackageVolume; }

            set
            {
                if (!Equals(_currentPackageVolume, value))
                {
                    _currentPackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPackageVolume)));
                }
            }
        }

        private PackageVolumeModel _selectedPackageVolume;

        public PackageVolumeModel SelectedPackageVolume
        {
            get { return _selectedPackageVolume; }

            set
            {
                if (!Equals(_selectedPackageVolume, value))
                {
                    _selectedPackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPackageVolume)));
                }
            }
        }

        public ObservableCollection<PackageVolumeModel> PackageVolumeCollection = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PackageVolumeDialog()
        {
            InitializeComponent();
        }
    }
}
