using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：历史记录设置控件
    /// </summary>
    public sealed partial class HistoryRecordControl : Grid, INotifyPropertyChanged
    {
        private GroupOptionsModel _historyLiteItem = HistoryRecordService.HistoryLiteNum;

        public GroupOptionsModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set
            {
                _historyLiteItem = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> HistoryLiteNumList { get; } = HistoryRecordService.HistoryLiteNumList;

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryRecordControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 微软商店页面“历史记录”显示数目修改
        /// </summary>
        public void OnHistoryLiteSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                HistoryLiteItem = HistoryLiteNumList[Convert.ToInt32(item.Tag)];
                HistoryRecordService.SetHistoryLiteNum(HistoryLiteItem);
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
