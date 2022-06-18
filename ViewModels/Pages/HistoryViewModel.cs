using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.App;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Pages
{
    public class HistoryViewModel : ObservableRecipient
    {
        private bool _isEditMode;

        public bool IsEditMode
        {
            get { return _isEditMode; }

            set { SetProperty(ref _isEditMode, value); }
        }

        private HistoryModel _selectedHistoryItem;

        public HistoryModel SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }

            set { SetProperty(ref _selectedHistoryItem, value); }
        }

        public IAsyncRelayCommand LoadedCommand { get; set; }

        public ICommand CopyCommand { get; set; }

        public IAsyncRelayCommand FillinCommand { get; set; }

        public ICommand SelectCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public List<GetAppTypeModel> TypeList { get; } = new List<GetAppTypeModel>
        {
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("URL"),InternalName="url"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("ProductID"),InternalName="ProductId"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("PackageFamilyName"),InternalName="PackageFamilyName"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("CategoryID"),InternalName="CategoryId"}
        };

        public List<GetAppChannelModel> ChannelList { get; } = new List<GetAppChannelModel>
        {
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Fast"),InternalName="WIF" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Slow"),InternalName="WIS" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("RP"),InternalName="RP" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Retail"),InternalName="Retail" }
        };

        public ObservableCollection<HistoryModel> HistoryList = new ObservableCollection<HistoryModel>();

        public HistoryViewModel()
        {
            // List列表初始化，可以从数据库获得的列表中加载
            LoadedCommand = new AsyncRelayCommand(UpdateHistoryListAsync);

            CopyCommand = new AsyncRelayCommand(CopyAsync);

            FillinCommand = new AsyncRelayCommand(FillinAsync);

            SelectCommand = new RelayCommand(() => { IsEditMode = true; });

            DeleteCommand = new RelayCommand(() => { IsEditMode = false; });

            CancelCommand = new RelayCommand(() => { IsEditMode = false; });
        }

        /// <summary>
        /// UI加载完成时/或者是数据库数据发生变化时，从数据库中异步加载数据
        /// </summary>
        private async Task UpdateHistoryListAsync()
        {
            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = await HistoryDataService.QueryAllHistoryDataAsync();

            // 更新UI上面的数据
            UpdateList(HistoryRawList);
        }

        /// <summary>
        /// 更新UI上面的数据
        /// </summary>
        private void UpdateList(List<HistoryModel> historyRawList)
        {
            HistoryList.Clear();

            foreach (HistoryModel historyRawData in historyRawList)
            {
                HistoryList.Add(historyRawData);
            }
        }

        /// <summary>
        /// 将选中的历史记录条目填入到选择控件中，然后点击“获取链接”即可获取
        /// </summary>
        private async Task FillinAsync()
        {
            if (SelectedHistoryItem == null) return;

            Messenger.Send(new FillinMessage(SelectedHistoryItem));
            await Task.CompletedTask;
        }

        private async Task CopyAsync()
        {
            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(SelectedHistoryItem.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(SelectedHistoryItem.HistoryChannel)).DisplayName,
                SelectedHistoryItem.HistoryLink);
            CopyPasteService.CopyStringToClicpBoard(CopyContent);

            await Task.CompletedTask;
        }
    }
}
