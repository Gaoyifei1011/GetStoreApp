using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class TraceCleanupPromptViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private ITraceCleanupService TraceCleanupService { get; } = IOCHelper.GetService<ITraceCleanupService>();

        public List<TraceCleanupModel> TraceCleanupList { get; set; }

        private bool _isFirstInitialize = true;

        public bool IsFirstInitialize
        {
            get { return _isFirstInitialize; }

            set { SetProperty(ref _isFirstInitialize, value); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set { SetProperty(ref _isSelected, value); }
        }

        private bool _isCleaning = false;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set { SetProperty(ref _isCleaning, value); }
        }

        public void InitializeTraceCleanupList()
        {
            TraceCleanupList = ResourceService.TraceCleanupList;

            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsSelected = false;
                traceCleanupItem.IsCleanFailed = false;
                traceCleanupItem.PropertyChanged += OnPropertyChanged;
            }
        }

        // 痕迹清理
        public IRelayCommand TraceCleanupSureCommand => new RelayCommand(async () =>
        {
            IsFirstInitialize = false;
            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsCleanFailed = false;
            }
            IsCleaning = true;
            await TraceCleanupAsync();
            await Task.Delay(1000);
            IsCleaning = false;
        });

        // 取消痕迹清理
        public IRelayCommand TraceCleanupCancelCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.PropertyChanged -= OnPropertyChanged;
            }
            dialog.Hide();
        });

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private async Task TraceCleanupAsync()
        {
            List<CleanArgs> SelectedCleanList = new List<CleanArgs>(TraceCleanupList.Where(item => item.IsSelected == true).Select(item => item.InternalName));

            foreach (CleanArgs cleanupArgs in SelectedCleanList)
            {
                // 清理并反馈回结果，修改相应的状态信息
                bool CleanReusult = await TraceCleanupService.CleanAppTraceAsync(cleanupArgs);

                if (cleanupArgs == CleanArgs.History)
                {
                    WeakReferenceMessenger.Default.Send(new HistoryMessage(true));
                }

                TraceCleanupList[TraceCleanupList.IndexOf(TraceCleanupList.First(item => item.InternalName == cleanupArgs))].IsCleanFailed = !CleanReusult;
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
        }
    }
}
