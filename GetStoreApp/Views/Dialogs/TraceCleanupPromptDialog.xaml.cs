using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 痕迹清理对话框
    /// </summary>
    public sealed partial class TraceCleanupPromptDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string ActionCenterErrorString = ResourceService.GetLocalized("Dialog/ActionCenterError");
        private readonly string ActionCenterString = ResourceService.GetLocalized("Dialog/ActionCenter");
        private readonly string DownloadCleanErrorString = ResourceService.GetLocalized("Dialog/DownloadCleanError");
        private readonly string DownloadRecordString = ResourceService.GetLocalized("Dialog/DownloadRecord");
        private readonly string HistoryCleanErrorString = ResourceService.GetLocalized("Dialog/HistoryCleanError");
        private readonly string HistoryRecordString = ResourceService.GetLocalized("Dialog/HistoryRecord");
        private readonly string LocalFileString = ResourceService.GetLocalized("Dialog/LocalFile");
        private readonly string LocalFileCleanErrorString = ResourceService.GetLocalized("Dialog/LocalFileCleanError");

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private bool _isCleaning;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                if (!Equals(_isCleaning, value))
                {
                    _isCleaning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCleaning)));
                }
            }
        }

        private List<TraceCleanupModel> TraceCleanupList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public TraceCleanupPromptDialog()
        {
            InitializeComponent();

            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsSelected = false,
                IsCleanFailed = false,
                DisplayName = HistoryRecordString,
                InternalName = CleanKind.History,
                CleanFailedText = HistoryCleanErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsSelected = false,
                IsCleanFailed = false,
                DisplayName = ActionCenterString,
                InternalName = CleanKind.ActionCenter,
                CleanFailedText = ActionCenterErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsSelected = false,
                IsCleanFailed = false,
                DisplayName = DownloadRecordString,
                InternalName = CleanKind.Download,
                CleanFailedText = DownloadCleanErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsSelected = false,
                IsCleanFailed = false,
                DisplayName = LocalFileString,
                InternalName = CleanKind.LocalFile,
                CleanFailedText = LocalFileCleanErrorString
            });

            IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private async void OnCleanupNowClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsCleanFailed = false;
            }

            IsCleaning = true;
            List<(CleanKind cleanKind, bool cleanResult)> cleanSuccessfullyDict = [];

            await Task.Run(async () =>
            {
                List<CleanKind> selectedCleanList = [];

                foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
                {
                    if (traceCleanupItem.IsSelected)
                    {
                        selectedCleanList.Add(traceCleanupItem.InternalName);
                    }
                }

                foreach (CleanKind cleanArgs in selectedCleanList)
                {
                    // 清理并反馈回结果，修改相应的状态信息
                    bool cleanReusult = TraceCleanupService.CleanAppTraceAsync(cleanArgs);
                    cleanSuccessfullyDict.Add(ValueTuple.Create(cleanArgs, cleanReusult));
                }

                await Task.Delay(1000);
            });

            foreach ((CleanKind cleanKind, bool cleanResult) in cleanSuccessfullyDict)
            {
                foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
                {
                    if (Equals(traceCleanupItem.InternalName, cleanKind))
                    {
                        traceCleanupItem.IsCleanFailed = !cleanResult;
                        break;
                    }
                }
            }

            IsCleaning = false;
        }

        /// <summary>
        /// 复选框选中时的状态发生更改时触发的事件
        /// </summary>
        private void OnCheckBoxExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
        }
    }
}
