using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class HistoryCopyNotification : UserControl, INotifyPropertyChanged
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private int _historyCopyState;

        public int HistoryCopyState
        {
            get { return _historyCopyState; }

            set
            {
                _historyCopyState = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HistoryCopyState)));
            }
        }

        public HistoryCopyNotification()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
