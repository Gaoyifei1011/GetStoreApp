using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：历史记录设置控件
    /// </summary>
    public sealed partial class HistoryRecordControl : Expander, INotifyPropertyChanged
    {
        private HistoryLiteNumModel _historyLiteItem = HistoryRecordService.HistoryLiteNum;

        public HistoryLiteNumModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set
            {
                _historyLiteItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HistoryLiteItem)));
            }
        }

        private HistoryJumpListNumModel _historyJumpListItem = HistoryRecordService.HistoryJumpListNum;

        public HistoryJumpListNumModel HistoryJumpListItem
        {
            get { return _historyJumpListItem; }

            set
            {
                _historyJumpListItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HistoryJumpListItem)));
            }
        }

        public List<HistoryLiteNumModel> HistoryLiteNumList { get; } = HistoryRecordService.HistoryLiteNumList;

        public List<HistoryJumpListNumModel> HistoryJumpListNumList { get; } = HistoryRecordService.HistoryJumpListNumList;

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryRecordControl()
        {
            InitializeComponent();
        }

        public bool IsHistoryLiteItemChecked(int selectedIHistoryLiteNumValue, int historyLiteNumValue)
        {
            return selectedIHistoryLiteNumValue == historyLiteNumValue;
        }

        public bool IsHistoryJumpListItemChecked(string selectedIHistoryJumpListNumValue, string historyJumpListNumValue)
        {
            return selectedIHistoryJumpListNumValue == historyJumpListNumValue;
        }

        /// <summary>
        /// 微软商店页面“历史记录”显示数目修改
        /// </summary>
        public async void OnHistoryLiteSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender.As<RadioMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                HistoryLiteItem = HistoryLiteNumList[Convert.ToInt32(item.Tag)];
                await HistoryRecordService.SetHistoryLiteNumAsync(HistoryLiteItem);
            }
        }

        /// <summary>
        /// 任务栏右键菜单列表“历史记录”显示数目修改
        /// </summary>
        public async void OnHistoryJumpListSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender.As<RadioMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                HistoryJumpListItem = HistoryJumpListNumList[Convert.ToInt32(item.Tag)];
                await HistoryRecordService.SetHistoryJumpListNumAsync(HistoryJumpListItem);
                await HistoryRecordService.UpdateHistoryJumpListAsync(HistoryJumpListItem);
            }
        }
    }
}
