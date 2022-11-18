using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Messages;
using GetStoreApp.Models.Dialogs.CommonDialogs.Settings;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public sealed class TraceCleanupPromptViewModel : ViewModelBase
    {
        public List<TraceCleanupModel> TraceCleanupList { get; set; }

        private bool _isFirstInitialize = true;

        public bool IsFirstInitialize
        {
            get { return _isFirstInitialize; }

            set
            {
                _isFirstInitialize = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isCleaning = false;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                _isCleaning = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 初始化清理列表信息
        /// </summary>
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
        public IRelayCommand CleanupNowCommand => new RelayCommand(async () =>
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

        // 关闭窗口
        public IRelayCommand CloseWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
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
