using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Home;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class RequestViewModel : ObservableRecipient
    {
        private bool BlockMapFilterValue { get; set; } = LinkFilterService.LinkFilterValue[1];

        private bool StartsWithEFilterValue { get; set; } = LinkFilterService.LinkFilterValue[0];

        public string CategoryId { get; } = LanguageService.GetResources("/Home/CategoryId");

        private string RegionCodeName { get; set; } = RegionService.RegionCodeName;

        public string ResultCountInfo { get; } = LanguageService.GetResources("/Home/ResultCountInfo");

        private string SampleTitle { get; } = LanguageService.GetResources("/Home/SampleTitle");

        private string SampleLink { get; set; }

        private HomeType _selectedType;

        public HomeType SelectedType
        {
            get { return _selectedType; }

            set { SetProperty(ref _selectedType, value); }
        }

        private HomeChannel _selectedChannel;

        public HomeChannel SelectedChannel
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

        private ICommand _typeSelectionChangedCommand;

        public ICommand TypeSelectionChangedCommand
        {
            get { return _typeSelectionChangedCommand; }

            set { SetProperty(ref _typeSelectionChangedCommand, value); }
        }

        private ICommand _getLinksCommand;

        public ICommand GetLinksCommand
        {
            get { return _getLinksCommand; }

            set { SetProperty(ref _getLinksCommand, value); }
        }

        public IReadOnlyList<HomeType> TypeList { get; } = new List<HomeType>()
        {
            new HomeType(){DisplayName=LanguageService.GetResources("/Home/TypeURL"),InternalName="url"},
            new HomeType(){DisplayName=LanguageService.GetResources("/Home/TypePID"),InternalName="ProductId"},
            new HomeType(){DisplayName=LanguageService.GetResources("/Home/TypePFN"),InternalName="PackageFamilyName"},
            new HomeType(){DisplayName=LanguageService.GetResources("/Home/TypeCID"),InternalName="CategoryId"}
        };

        public IReadOnlyList<HomeChannel> ChannelList { get; } = new List<HomeChannel>()
        {
            new HomeChannel(){ DisplayName=LanguageService.GetResources("/Home/ChannelFast"),InternalName="WIF" },
            new HomeChannel(){ DisplayName=LanguageService.GetResources("/Home/ChannelSlow"),InternalName="WIS" },
            new HomeChannel(){ DisplayName=LanguageService.GetResources("/Home/ChannelRP"),InternalName="RP" },
            new HomeChannel(){ DisplayName=LanguageService.GetResources("/Home/ChannelRetail"),InternalName="Retail" }
        };

        public static IReadOnlyList<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };

        public RequestViewModel()
        {
            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            TypeSelectionChangedCommand = new RelayCommand(SetPlaceHolderText);

            GetLinksCommand = new RelayCommand(async () => { await GetLinksClickedAsync(); });

            SelectedType = TypeList[0];

            SelectedChannel = ChannelList[3];

            Messenger.Register<RequestViewModel, RegionMessage>(this, (requestViewModel, regionMessage) =>
            {
                requestViewModel.RegionCodeName = regionMessage.Value;
            });

            Messenger.Register<RequestViewModel, StartsWithEFilterMessage>(this, (requestViewModel, startsWithEFilterMessage) =>
            {
                requestViewModel.StartsWithEFilterValue = startsWithEFilterMessage.Value;
            });

            Messenger.Register<RequestViewModel, BlockMapFilterMessage>(this, (requestViewModel, blockMapFilterService) =>
            {
                requestViewModel.BlockMapFilterValue = blockMapFilterService.Value;
            });
        }

        private void SetPlaceHolderText()
        {
            if (SelectedType.InternalName == TypeList[0].InternalName)
            {
                SampleLink = SampleLinkList[0];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[1].InternalName)
            {
                SampleLink = SampleLinkList[1];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[2].InternalName)
            {
                SampleLink = SampleLinkList[2];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[3].InternalName)
            {
                SampleLink = SampleLinkList[3];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else
            {
                SampleLink = string.Empty;
                LinkPlaceHolderText = string.Empty;
            }

            LinkText = string.Empty;
        }

        private async Task GetLinksClickedAsync()
        {
            bool ResultControlVisable;

            string CategoryId = string.Empty;

            List<ResultData> ResultDataList = new();

            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 设置获取数据时的相关控件状态
            Messenger.Send(new StatusBarStateMessage(0));

            // 生成请求的内容
            GenerateContentService generateContentService = new();
            string content = generateContentService.GenerateContent(SelectedType.InternalName, LinkText, SelectedChannel.InternalName, RegionCodeName);

            // 获取网页反馈回的原始数据
            HtmlRequestService htmlRequestService = new HtmlRequestService();
            HttpRequestData httpRequestData = await htmlRequestService.HttpRequestAsync(content);

            // 检查服务器返回获取的状态
            HtmlRequestStateService htmlRequestStateService = new HtmlRequestStateService();
            int state = htmlRequestStateService.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            // 成功状态下解析数据
            if (state == 1)
            {
                HtmlParseService htmlParseService = new(httpRequestData);

                CategoryId = htmlParseService.HtmlParseCID();

                ResultDataList = htmlParseService.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);
            }

            // 发送消息
            Messenger.Send(new StatusBarStateMessage(state));

            Messenger.Send(new ResultControlVisableMessage(ResultControlVisable));

            Messenger.Send(new ResultCategoryIdMessage(CategoryId));

            Messenger.Send(new ResultDataListMessage(ResultDataList));
        }

        private void ResultListFilter(ref List<ResultData> resultDataList)
        {
            // 按要求过滤列表内容
            if (StartsWithEFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
