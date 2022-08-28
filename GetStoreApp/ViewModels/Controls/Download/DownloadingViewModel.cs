using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class DownloadingViewModel : ObservableRecipient
    {
        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private DispatcherTimer DownloadingTimer { get; } = new DispatcherTimer();

        public ObservableCollection<DownloadModel> DownloadingDataList { get; } = new ObservableCollection<DownloadModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        public IAsyncRelayCommand PauseAllCommand { get; }

        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            await SelectNoneAsync();
            IsSelectMode = true;
        });

        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadModel downloadItem in DownloadingDataList)
            {
                downloadItem.IsSelected = true;
            }
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(SelectNoneAsync);

        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(DeleteSelected);

        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand PauseCommand { get; }

        public IAsyncRelayCommand DeleteCommand { get; }

        public DownloadingViewModel()
        {
            // Tick 超过计时器间隔时发生
            DownloadingTimer.Tick += DownloadInfoTimerTick;
            // Interval 获取或设置计时器刻度之间的时间段
            DownloadingTimer.Interval = new TimeSpan(0, 0, 1);

            Messenger.Register<DownloadingViewModel, PivotSelectionMessage>(this, async (downloadingViewModel, pivotSelectionMessage) =>
            {
                // 切换到下载中页面时，开启监控。并更新当前页面的数据
                if (pivotSelectionMessage.Value == 0)
                {
                    DownloadingTimer.Start();
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    if (DownloadingTimer.IsEnabled)
                    {
                        DownloadingTimer.Stop();
                    }
                    DownloadingTimer.Tick -= DownloadInfoTimerTick;
                    Messenger.UnregisterAll(this);
                }

                // 切换到其他页面时，关闭监控
                else
                {
                    DownloadingTimer.Stop();
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 计时器：获取正在下载中的文件信息
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
        }

        /// <summary>
        /// 删除选中的下载任务
        /// </summary>
        private async Task DeleteSelected()
        {
            List<DownloadModel> SelectedDownloadomgDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadomgDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                //await IDownloadDBService.DeleteDownloadDataAsync(SelectedDownloadomgDataList);

                //// 如果有正在下载的服务，从下载列表中删除
                //if (DownloadingList.Any())
                //{
                //    foreach (DownloadModel downloadItem in DownloadingList)
                //    {
                //        //Aria2Service.DeleteSelectedAsync();
                //    }
                //}
                await GetDownloadDataListAsync();
            }
        }

        /// <summary>
        /// 从数据库中加载还未下载完成的数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            //Tuple<List<DownloadModel>, bool> DownloadData = await IDownloadDBService.QueryDownloadDataAsync();

            //List<DownloadModel> DownloadRawList = DownloadData.Item1;

            //IsDownloadingEmpty = DownloadData.Item2;

            //try
            //{
            //    ConvertRawListToDisplayList(ref DownloadRawList);
            //}
            //catch (Exception)
            //{
            //    ConvertRawListToDisplayList(ref DownloadRawList);
            //}
            await Task.CompletedTask;
        }

        /// <summary>
        /// 将原始数据转换为在UI界面上呈现出来的数据
        /// </summary>
        private void ConvertRawListToDisplayList(ref List<DownloadModel> downloadRawList)
        {
            DownloadingDataList.Clear();

            foreach (DownloadModel downloadRawData in downloadRawList)
            {
                DownloadingDataList.Add(downloadRawData);
            }
        }

        /// <summary>
        /// 点击全部不选按钮时，让复选框的下载记录全部不选
        /// </summary>
        private async Task SelectNoneAsync()
        {
            foreach (DownloadModel downloadItem in DownloadingDataList)
            {
                downloadItem.IsSelected = false;
            }

            await Task.CompletedTask;
        }
    }
}
