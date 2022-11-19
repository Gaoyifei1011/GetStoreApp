using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Pages
{
    public sealed class HistoryViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object HistoryDataListLock = new object();

        public List<TypeModel> TypeList => ResourceService.TypeList;

        public List<ChannelModel> ChannelList => ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryDataList { get; } = new ObservableCollection<HistoryModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        private bool _isHistoryEmpty = true;

        public bool IsHistoryEmpty
        {
            get { return _isHistoryEmpty; }

            set
            {
                _isHistoryEmpty = value;
                OnPropertyChanged();
            }
        }

        private bool _isHistoryEmptyAfterFilter = true;

        public bool IsHistoryEmptyAfterFilter
        {
            get { return _isHistoryEmptyAfterFilter; }

            set
            {
                _isHistoryEmptyAfterFilter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 时间排序的规则，false为递减排序，true为递增排序
        /// </summary>
        private bool _timeSortOrder = false;

        public bool TimeSortOrder
        {
            get { return _timeSortOrder; }

            set
            {
                _timeSortOrder = value;
                OnPropertyChanged();
            }
        }

        private string _typeFilter = "None";

        public string TypeFilter
        {
            get { return _typeFilter; }

            set
            {
                _typeFilter = value;
                OnPropertyChanged();
            }
        }

        private string _channelFilter = "None";

        public string ChannelFilter
        {
            get { return _channelFilter; }

            set
            {
                _channelFilter = value;
                OnPropertyChanged();
            }
        }

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
        });

        // 按时间进行排序
        public IRelayCommand TimeSortCommand => new RelayCommand<string>(async (value) =>
        {
            TimeSortOrder = Convert.ToBoolean(value);
            await GetHistoryDataListAsync();
        });

        // 按类型进行过滤
        public IRelayCommand TypeFilterCommand => new RelayCommand<string>(async (value) =>
        {
            TypeFilter = value;
            await GetHistoryDataListAsync();
        });

        // 按通道进行过滤
        public IRelayCommand ChannelFilterCommand => new RelayCommand<string>(async (value) =>
        {
            ChannelFilter = value;
            await GetHistoryDataListAsync();
        });

        // 刷新数据
        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            await GetHistoryDataListAsync();
        });

        // 全选
        public IRelayCommand SelectAllCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = true;
                }
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }
        });

        // 复制选定项目的内容
        public IRelayCommand CopySelectedCommand => new RelayCommand(async () =>
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected == true).ToList();

            if (SelectedHistoryDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            foreach (HistoryModel selectedHistoryData in SelectedHistoryDataList)
            {
                stringBuilder.AppendLine(string.Format("{0}\t{1}\t{2}",
                    TypeList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryType)).DisplayName,
                    ChannelList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryChannel)).DisplayName,
                    selectedHistoryData.HistoryLink));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new HistoryCopyNotification(true, true, SelectedHistoryDataList.Count).Show();
        });

        // 删除选定的项目
        public IRelayCommand DeleteCommand => new RelayCommand(async () =>
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedHistoryDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeleteArgs.History).ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteResult = await HistoryDBService.DeleteAsync(SelectedHistoryDataList);

                if (DeleteResult)
                {
                    // 确保线程安全
                    lock (HistoryDataListLock)
                    {
                        foreach (HistoryModel historyItem in SelectedHistoryDataList)
                        {
                            HistoryDataList.Remove(historyItem);
                        }
                    }
                }

                if (HistoryDataList.Count == 0)
                {
                    if (TypeFilter == "None" || ChannelFilter == "None")
                    {
                        IsHistoryEmpty = true;
                        IsHistoryEmptyAfterFilter = false;
                    }
                    else
                    {
                        IsHistoryEmpty = false;
                        IsHistoryEmptyAfterFilter = true;
                    }
                }

                Messenger.Default.Send(true, MessageToken.History);
            }
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 填入指定项目的内容
        public IRelayCommand FillinCommand => new RelayCommand<HistoryModel>((historyItem) =>
        {
            App.NavigationArgs = AppNaviagtionArgs.Home;
            Messenger.Default.Send(historyItem, MessageToken.Fillin);
            NavigationService.NavigateTo(typeof(HomePage));
        });

        // 复制指定项目的内容
        public IRelayCommand CopyCommand => new RelayCommand<HistoryModel>((historyItem) =>
        {
            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                historyItem.HistoryLink);
            CopyPasteHelper.CopyToClipBoard(CopyContent);

            new HistoryCopyNotification(true, false).Show();
        });

        // 导航到历史记录页面时，历史记录数据列表初始化，从数据库中存储的列表中加载
        public async void OnNavigatedTo()
        {
            await GetHistoryDataListAsync();
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public void OnItemClick(object sender, ItemClickEventArgs args)
        {
            HistoryModel historyItem = (HistoryModel)args.ClickedItem;
            int ClickedIndex = HistoryDataList.IndexOf(historyItem);

            lock (HistoryDataListLock)
            {
                HistoryDataList[ClickedIndex].IsSelected = !HistoryDataList[ClickedIndex].IsSelected;
            }
        }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetHistoryDataListAsync()
        {
            (List<HistoryModel>, bool, bool) HistoryAllData = await HistoryDBService.QueryAllAsync(TimeSortOrder, TypeFilter, ChannelFilter);

            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = HistoryAllData.Item1;
            // 数据库中的历史记录表是否为空
            IsHistoryEmpty = HistoryAllData.Item2;

            // 经过筛选后历史记录是否为空
            IsHistoryEmptyAfterFilter = HistoryAllData.Item3;

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                HistoryDataList.Clear();
            }

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyRawData in HistoryRawList)
                {
                    HistoryDataList.Add(historyRawData);
                }
            }
        }
    }
}
