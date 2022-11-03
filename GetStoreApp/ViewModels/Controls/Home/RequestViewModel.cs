using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.History;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Controls.Home;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Home;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class RequestViewModel : ObservableRecipient
    {
        private IRegionService RegionService { get; } = ContainerHelper.GetInstance<IRegionService>();

        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private IStartupArgsService StartupArgsService { get; } = ContainerHelper.GetInstance<IStartupArgsService>();

        private IHistoryDBService HistoryDBService { get; } = ContainerHelper.GetInstance<IHistoryDBService>();

        private ILinkFilterService LinkFilterService { get; } = ContainerHelper.GetInstance<ILinkFilterService>();

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

            set { SetProperty(ref _selectedType, value); }
        }

        private int _selectedChannel = 3;

        public int SelectedChannel
        {
            get { return _selectedChannel; }

            set { SetProperty(ref _selectedChannel, value); }
        }

        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set { SetProperty(ref _linkPlaceHolderText, value); }
        }

        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set { SetProperty(ref _linkText, value); }
        }

        private bool _isGettingLinks;

        public bool IsGettingLinks
        {
            get { return _isGettingLinks; }

            set { SetProperty(ref _isGettingLinks, value); }
        }

        // 类型选择后修改样例文本
        public IRelayCommand TypeSelectCommand => new RelayCommand(() =>
        {
            SampleLink = SampleLinkList[SelectedType];
            LinkPlaceHolderText = SampleTitle + SampleLink;

            LinkText = string.Empty;
        });

        // 获取链接
        public IRelayCommand GetLinksCommand => new RelayCommand(async () =>
        {
            await GetLinksAsync();
        });

        public RequestViewModel()
        {
            SelectedType = Convert.ToInt32(StartupArgsService.StartupArgs["TypeName"]) == -1 ? 0 : Convert.ToInt32(StartupArgsService.StartupArgs["TypeName"]);

            SelectedChannel = Convert.ToInt32(StartupArgsService.StartupArgs["ChannelName"]) == -1 ? 3 : Convert.ToInt32(StartupArgsService.StartupArgs["ChannelName"]);

            LinkText = StartupArgsService.StartupArgs["Link"] is null ? string.Empty : (string)StartupArgsService.StartupArgs["Link"];

            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            IsGettingLinks = false;

            App.MainWindow.Closed += OnWindowClosed;

            WeakReferenceMessenger.Default.Register<RequestViewModel, FillinMessage>(this, (requestViewModel, fillinMessage) =>
            {
                requestViewModel.SelectedType = TypeList.FindIndex(item => item.InternalName.Equals(fillinMessage.Value.HistoryType));
                requestViewModel.SelectedChannel = ChannelList.FindIndex(item => item.InternalName.Equals(fillinMessage.Value.HistoryChannel));
                requestViewModel.LinkText = fillinMessage.Value.HistoryLink;
            });
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
            WeakReferenceMessenger.Default.Send(new StatusBarStateMessage(0));

            // 生成请求的内容
            GenerateContentHelper generateContentService = new GenerateContentHelper();
            string content = generateContentService.GenerateContent(
                TypeList[SelectedType].InternalName,
                LinkText,
                ChannelList[SelectedChannel].InternalName,
                RegionService.AppRegion.ISO2);

            // 获取网页反馈回的原始数据
            HtmlRequestHelper htmlRequestService = new HtmlRequestHelper();
            RequestModel httpRequestData = await htmlRequestService.HttpRequestAsync(content);

            // 检查服务器返回获取的状态
            HtmlRequestStateHelper htmlRequestStateService = new HtmlRequestStateHelper();
            int state = htmlRequestStateService.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            IsGettingLinks = false;

            // 成功状态下解析数据，并更新相应的历史记录
            if (state == 1)
            {
                HtmlParseHelper htmlParseService = new HtmlParseHelper(httpRequestData);

                CategoryId = htmlParseService.HtmlParseCID();

                ResultDataList = htmlParseService.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);

                await UpdateHistoryAsync(CurrentType, CurrentChannel, CurrentLink);

                WeakReferenceMessenger.Default.Send(new HistoryMessage(true));
            }

            // 发送消息
            WeakReferenceMessenger.Default.Send(new StatusBarStateMessage(state));

            WeakReferenceMessenger.Default.Send(new ResultControlVisableMessage(ResultControlVisable));

            WeakReferenceMessenger.Default.Send(new ResultCategoryIdMessage(CategoryId));

            WeakReferenceMessenger.Default.Send(new ResultDataListMessage(ResultDataList));
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

        private void ResultListFilter(ref List<ResultModel> resultDataList)
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

        /// <summary>
        /// 应用关闭后注销所有消息服务，释放所有资源
        /// </summary>
        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            App.MainWindow.Closed -= OnWindowClosed;
        }
    }
}
