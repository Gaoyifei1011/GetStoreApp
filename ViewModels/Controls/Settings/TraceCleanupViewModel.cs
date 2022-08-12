using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class TraceCleanupViewModel : ObservableRecipient
    {
        private int CountDownTime { get; set; } = 3;

        private DispatcherTimer ClearRecordTimer { get; } = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

        private IHistoryDataService HistoryDataService { get; } = IOCHelper.GetService<IHistoryDataService>();

        private IDownloadDataService DownloadDataService { get; } = IOCHelper.GetService<IDownloadDataService>();


        private bool _clearState = false;

        public bool ClearState
        {
            get { return _clearState; }

            set { SetProperty(ref _clearState, value); }
        }

        private bool _clearTextVisValue = false;

        public bool ClearTextVisValue
        {
            get { return _clearTextVisValue; }

            set { SetProperty(ref _clearTextVisValue, value); }
        }

        private string _testText;

        public string TestText
        {
            get { return _testText; }

            set { SetProperty(ref _testText, value); }
        }

        public IAsyncRelayCommand ClearRecordCommand { get; set; }

        public TraceCleanupViewModel()
        {
            ClearRecordCommand = new AsyncRelayCommand(ClearRecordAsync);

            ClearRecordTimer.Tick += ClearRecordTimerTick;
        }

        private async Task ClearRecordAsync()
        {
            // 添加删除下载记录（包括文件）对话框

            bool HistoryClearResult = await HistoryDataService.ClearHistoryDataAsync();
            bool DownloadClearResult = await DownloadDataService.ClearDownloadDataAsync();

            // 清理完成后显示提示文字
            ClearState = HistoryClearResult;
            ClearTextVisValue = true;

            ClearRecordTimer.Start();

            Messenger.Send(new HistoryMessage(true));
        }

        private void ClearRecordTimerTick(object sender, object args)
        {
            if (CountDownTime > 0) CountDownTime--;
            else
            {
                ClearRecordTimer.Stop();
                CountDownTime = 3;
                ClearTextVisValue = false;
            }
        }
    }
}
