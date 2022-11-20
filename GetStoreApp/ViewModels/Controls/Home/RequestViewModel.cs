using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Controls.Home;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public sealed class RequestViewModel : ViewModelBase
    {
        public List<TypeModel> TypeList => ResourceService.TypeList;

        public List<ChannelModel> ChannelList => ResourceService.ChannelList;

        public static IReadOnlyList<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };

        private string SampleTitle => ResourceService.GetLocalized("/Home/SampleTitle");

        private string SampleLink { get; set; }

        private int _selectedType;

        public int SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        private int _selectedChannel = 3;

        public int SelectedChannel
        {
            get { return _selectedChannel; }

            set
            {
                _selectedChannel = value;
                OnPropertyChanged();
            }
        }

        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set
            {
                _linkPlaceHolderText = value;
                OnPropertyChanged();
            }
        }

        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set
            {
                _linkText = value;
                OnPropertyChanged();
            }
        }

        private bool _isGettingLinks;

        public bool IsGettingLinks
        {
            get { return _isGettingLinks; }

            set
            {
                _isGettingLinks = value;
                OnPropertyChanged();
            }
        }

        // 获取链接
        public IRelayCommand GetLinksCommand => new RelayCommand(async () =>
        {
            await GetLinksAsync();
        });

        public RequestViewModel()
        {
            SelectedType = Convert.ToInt32(StartupService.StartupArgs["TypeName"]) == -1 ? 0 : Convert.ToInt32(StartupService.StartupArgs["TypeName"]);

            SelectedChannel = Convert.ToInt32(StartupService.StartupArgs["ChannelName"]) == -1 ? 3 : Convert.ToInt32(StartupService.StartupArgs["ChannelName"]);

            LinkText = StartupService.StartupArgs["Link"] is null ? string.Empty : (string)StartupService.StartupArgs["Link"];

            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            IsGettingLinks = false;

            Messenger.Default.Register<string[]>(this, MessageToken.Command, (commandMessage) =>
            {
                SelectedType = Convert.ToInt32(commandMessage[0]) == -1 ? 0 : Convert.ToInt32(commandMessage[0]);
                SelectedChannel = Convert.ToInt32(commandMessage[1]) == -1 ? 3 : Convert.ToInt32(commandMessage[1]);
                LinkText = commandMessage[2] == "PlaceHolderText" ? string.Empty : commandMessage[2];

                if (NavigationService.GetCurrentPageType() != typeof(HomePage))
                {
                    NavigationService.NavigateTo(typeof(HomePage));
                }
            });

            Messenger.Default.Register<HistoryModel>(this, MessageToken.Fillin, (fillinMessage) =>
            {
                SelectedType = TypeList.FindIndex(item => item.InternalName.Equals(fillinMessage.HistoryType));
                SelectedChannel = ChannelList.FindIndex(item => item.InternalName.Equals(fillinMessage.HistoryChannel));
                LinkText = fillinMessage.HistoryLink;
            });

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    Messenger.Default.Unregister(this);
                }
            });
        }

        /// <summary>
        /// 类型选择后修改样例文本
        /// </summary>
        public void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                SampleLink = SampleLinkList[SelectedType];
                LinkPlaceHolderText = SampleTitle + SampleLink;

                LinkText = string.Empty;
            }
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await GetLinksAsync();
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        private async Task GetLinksAsync()
        {
            bool ResultControlVisable;

            string CategoryId = string.Empty;

            IsGettingLinks = true;

            List<ResultModel> ResultDataList = new List<ResultModel>();

            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 记录当前选定的选项和填入的内容
            int CurrentType = SelectedType;

            int CurrentChannel = SelectedChannel;

            string CurrentLink = LinkText;

            // 设置获取数据时的相关控件状态
            Messenger.Default.Send(0, MessageToken.StatusBarState);

            // 生成请求的内容
            string generateContent = new GenerateContentHelper().GenerateRequestContent(
                TypeList[SelectedType].InternalName,
                LinkText,
                ChannelList[SelectedChannel].InternalName,
                RegionService.AppRegion.ISO2);

            // 获取网页反馈回的原始数据
            HtmlRequestHelper htmlRequest = new HtmlRequestHelper();
            RequestModel httpRequestData = await htmlRequest.HttpRequestAsync(generateContent);

            // 检查服务器返回获取的状态
            HtmlRequestStateHelper htmlRequestState = new HtmlRequestStateHelper();
            int state = htmlRequestState.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            IsGettingLinks = false;

            // 成功状态下解析数据，并更新相应的历史记录
            if (state == 1)
            {
                HtmlParseHelper htmlParse = new HtmlParseHelper(httpRequestData);

                CategoryId = htmlParse.HtmlParseCID();

                ResultDataList = htmlParse.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);

                await UpdateHistoryAsync(CurrentType, CurrentChannel, CurrentLink);

                Messenger.Default.Send(true, MessageToken.History);
            }

            // 发送消息
            Messenger.Default.Send(state, MessageToken.StatusBarState);

            Messenger.Default.Send(ResultControlVisable, MessageToken.ResultControlVisable);

            Messenger.Default.Send(CategoryId, MessageToken.ResultCategoryId);

            Messenger.Default.Send(ResultDataList, MessageToken.ResultDataList);
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容和数据库中的内容
        /// </summary>
        private async Task UpdateHistoryAsync(int currentType, int currentChannel, string currentLink)
        {
            // 添加时间戳
            long TimeStamp = GenerateTimeStamp();

            await HistoryDBService.AddAsync(new HistoryModel
            {
                CreateTimeStamp = TimeStamp,
                HistoryKey = UniqueKeyHelper.GenerateHistoryKey(TypeList[currentType].InternalName, ChannelList[currentChannel].InternalName, currentLink),
                HistoryType = TypeList[currentType].InternalName,
                HistoryChannel = ChannelList[currentChannel].InternalName,
                HistoryLink = currentLink
            });
        }

        private long GenerateTimeStamp()
        {
            TimeSpan TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(TimeSpan.TotalSeconds);
        }

        private void ResultListFilter(ref List<GetStoreApp.Models.Controls.Home.ResultModel> resultDataList)
        {
            // 按要求过滤列表内容
            if (LinkFilterService.StartWithEFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
