using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Pages
{
    public class DownloadViewModel : ObservableRecipient
    {
        private StorageFolder DownloadFolder = ApplicationData.Current.LocalCacheFolder;

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        public ObservableCollection<DownloadModel> DownloadDataList { get; set; } = new ObservableCollection<DownloadModel>();

        public IAsyncRelayCommand OpenFolderCommand { get; set; }

        public IAsyncRelayCommand SelectCommand { get; set; }

        public IAsyncRelayCommand RefreshCommand { get; set; }

        public IAsyncRelayCommand SelectAllCommand { get; set; }

        public IAsyncRelayCommand SelectNoneCommand { get; set; }

        public IAsyncRelayCommand DeleteCommand { get; set; }

        public IAsyncRelayCommand DeleteWithFileCommand { get; set; }

        public IAsyncRelayCommand CancelCommand { get; set; }

        public DownloadViewModel()
        {
            OpenFolderCommand = new AsyncRelayCommand(async () =>
            {
                await Windows.System.Launcher.LaunchFolderAsync(DownloadFolder);
            });

            SelectCommand = new AsyncRelayCommand(async () =>
            {
                //await SelectNoneAsync();
                IsSelectMode = true;
                await Task.CompletedTask;
            });

            //RefreshCommand = new AsyncRelayCommand(GetHistoryDataListAsync);

            SelectAllCommand = new AsyncRelayCommand(async () =>
            {
                //foreach (var item in HistoryDataList) item.IsSelected = true;
                await Task.CompletedTask;
            });

            //SelectNoneCommand = new AsyncRelayCommand(SelectNoneAsync);

            //DeleteCommand = new AsyncRelayCommand(DeleteAsync);

            CancelCommand = new AsyncRelayCommand(async () =>
            {
                IsSelectMode = false;
                await Task.CompletedTask;
            });
        }
    }
}
