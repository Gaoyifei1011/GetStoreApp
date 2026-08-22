using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 痕迹清理对话框
    /// </summary>
    internal sealed partial class TraceCleanupPromptDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ActionCenterErrorString = ResourceService.GetLocalized("Dialog/ActionCenterError");
        private readonly string ActionCenterString = ResourceService.GetLocalized("Dialog/ActionCenter");
        private readonly string DownloadCleanErrorString = ResourceService.GetLocalized("Dialog/DownloadCleanError");
        private readonly string DownloadRecordString = ResourceService.GetLocalized("Dialog/DownloadRecord");
        private readonly string HistoryCleanErrorString = ResourceService.GetLocalized("Dialog/HistoryCleanError");
        private readonly string HistoryRecordString = ResourceService.GetLocalized("Dialog/HistoryRecord");
        private readonly string LocalFileString = ResourceService.GetLocalized("Dialog/LocalFile");
        private readonly string LocalFileCleanErrorString = ResourceService.GetLocalized("Dialog/LocalFileCleanError");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private bool _isItemSelected;

        private bool IsItemSelected
        {
            get { return _isItemSelected; }

            set
            {
                if (!Equals(_isItemSelected, value))
                {
                    _isItemSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsItemSelected)));
                }
            }
        }

        private bool _isCleaning;

        private bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                if (!Equals(_isCleaning, value))
                {
                    _isCleaning = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCleaning)));
                }
            }
        }

        private List<TraceCleanupModel> TraceCleanupList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal TraceCleanupPromptDialog()
        {
            InitializeComponent();
            InitializeData();
            IsItemSelected = TraceCleanupListView.SelectedItems.Count > 0;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 选中项发生变化时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            IsItemSelected = TraceCleanupListView.SelectedItems.Count > 0;
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

            List<TraceCleanupModel> selectedItemsList = GetSelectedItemsList();

            List<(CleanKind cleanKind, bool cleanResult)> cleanSuccessfullyDict = await TraceCleanupAsync(selectedItemsList);
            if (cleanSuccessfullyDict is not null)
            {
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
            }

            TraceCleanupListView.SelectedItems.Clear();
            IsCleaning = false;
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsCleanFailed = false,
                DisplayName = HistoryRecordString,
                InternalName = CleanKind.History,
                CleanFailedText = HistoryCleanErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsCleanFailed = false,
                DisplayName = ActionCenterString,
                InternalName = CleanKind.ActionCenter,
                CleanFailedText = ActionCenterErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsCleanFailed = false,
                DisplayName = DownloadRecordString,
                InternalName = CleanKind.Download,
                CleanFailedText = DownloadCleanErrorString
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                IsCleanFailed = false,
                DisplayName = LocalFileString,
                InternalName = CleanKind.LocalFile,
                CleanFailedText = LocalFileCleanErrorString
            });
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        private List<TraceCleanupModel> GetSelectedItemsList()
        {
            List<TraceCleanupModel> selectedItemsList = [];

            foreach (object traceCleanupItemObj in TraceCleanupListView.SelectedItems)
            {
                if (traceCleanupItemObj is TraceCleanupModel traceCleanupItem)
                {
                    selectedItemsList.Add(traceCleanupItem);
                }
            }

            return selectedItemsList;
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private async Task<List<(CleanKind cleanKind, bool cleanResult)>> TraceCleanupAsync(List<TraceCleanupModel> selectedItemsList)
        {
            if (selectedItemsList is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                List<(CleanKind cleanKind, bool cleanResult)> cleanSuccessfullyDict = [];
                List<CleanKind> selectedCleanList = [];

                foreach (TraceCleanupModel traceCleanupItem in selectedItemsList)
                {
                    selectedCleanList.Add(traceCleanupItem.InternalName);
                }

                foreach (CleanKind cleanArgs in selectedCleanList)
                {
                    // 清理并反馈回结果，修改相应的状态信息
                    bool cleanResult = TraceCleanupService.CleanAppTraceAsync(cleanArgs);
                    cleanSuccessfullyDict.Add(ValueTuple.Create(cleanArgs, cleanResult));
                }

                await Task.Delay(1000);
                return cleanSuccessfullyDict;
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
