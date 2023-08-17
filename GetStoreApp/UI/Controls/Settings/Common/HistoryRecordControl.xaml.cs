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
    public sealed partial class HistoryRecordControl : Expander, INotifyPropertyChanged
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

        private GroupOptionsModel _historyJumpListItem = HistoryRecordService.HistoryJumpListNum;

        public GroupOptionsModel HistoryJumpListItem
        {
            get { return _historyJumpListItem; }

            set
            {
                _historyJumpListItem = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> HistoryLiteNumList { get; } = HistoryRecordService.HistoryLiteNumList;

        public List<GroupOptionsModel> HistoryJumpListNumList { get; } = HistoryRecordService.HistoryJumpListNumList;

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryRecordControl()
        {
            InitializeComponent();
        }

        public bool IsHistoryLiteItemChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
        }

        public bool IsHistoryJumpListItemChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
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
        /// 任务栏右键菜单列表“历史记录”显示数目修改
        /// </summary>
        public async void OnHistoryJumpListSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                HistoryJumpListItem = HistoryJumpListNumList[Convert.ToInt32(item.Tag)];
                HistoryRecordService.SetHistoryJumpListNum(HistoryJumpListItem);
                await HistoryRecordService.UpdateHistoryJumpListAsync(HistoryJumpListItem);
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
