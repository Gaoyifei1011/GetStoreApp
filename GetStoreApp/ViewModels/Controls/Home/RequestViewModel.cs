using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Extensions.SystemTray;
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
using Windows.UI.StartScreen;

namespace GetStoreApp.ViewModels.Controls.Home
{
    /// <summary>
    /// 主页面：请求用户控件视图模型
    /// </summary>
    public sealed class RequestViewModel : ViewModelBase
    {
        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        public static List<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };

        private string SampleTitle { get; } = ResourceService.GetLocalized("Home/SampleTitle");

        private string SampleLink { get; set; }

        private TypeModel _selectedType;

        public TypeModel SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        private ChannelModel _selectedChannel;

        public ChannelModel SelectedChannel
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

        // 类型修改选择后修改样例文本
        public IRelayCommand TypeSelectCommand => new RelayCommand<string>((typeIndex) =>
        {
            SelectedType = TypeList[Convert.ToInt32(typeIndex)];
            SampleLink = SampleLinkList[TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName)];
            LinkPlaceHolderText = SampleTitle + SampleLink;

            LinkText = string.Empty;
        });

        // 通道选择修改
        public IRelayCommand ChannelSelectCommand => new RelayCommand<string>((channelIndex) =>
        {
            SelectedChannel = ChannelList[Convert.ToInt32(channelIndex)];
        });

        public RequestViewModel()
        {
            SelectedType = Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"])];

            SelectedChannel = Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"])];

            LinkText = DesktopLaunchService.LaunchArgs["Link"] is null ? string.Empty : (string)DesktopLaunchService.LaunchArgs["Link"];

            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            IsGettingLinks = false;

            Messenger.Default.Register<string[]>(this, MessageToken.Command, (commandMessage) =>
            {
                SelectedType = Convert.ToInt32(commandMessage[0]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(commandMessage[0])];
                SelectedChannel = Convert.ToInt32(commandMessage[1]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(commandMessage[1])];
                LinkText = commandMessage[2] is "PlaceHolderText" ? string.Empty : commandMessage[2];

                if (NavigationService.GetCurrentPageType() != typeof(HomePage))
                {
                    NavigationService.NavigateTo(typeof(HomePage));
                }
            });

            Messenger.Default.Register<HistoryModel>(this, MessageToken.Fillin, (fillinMessage) =>
            {
                SelectedType = TypeList.Find(item => item.InternalName.Equals(fillinMessage.HistoryType));
                SelectedChannel = ChannelList.Find(item => item.InternalName.Equals(fillinMessage.HistoryChannel));
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
            int CurrentType = TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName);

            int CurrentChannel = ChannelList.FindIndex(item => item.InternalName == SelectedChannel.InternalName);

            string CurrentLink = LinkText;

            // 设置获取数据时的相关控件状态
            Messenger.Default.Send(0, MessageToken.StatusBarState);

            // 生成请求的内容
            string generateContent = GenerateContentHelper.GenerateRequestContent(
                SelectedType.InternalName,
                LinkText,
                SelectedChannel.InternalName,
                RegionService.AppRegion.ISO2);

            // 获取网页反馈回的原始数据
            RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

            // 检查服务器返回获取的状态
            int state = HtmlRequestStateHelper.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            IsGettingLinks = false;

            // 成功状态下解析数据
            if (state is 1)
            {
                HtmlParseHelper.InitializeParseData(httpRequestData);

                CategoryId = HtmlParseHelper.HtmlParseCID();

                ResultDataList = HtmlParseHelper.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);

                Messenger.Default.Send(true, MessageToken.History);
            }

            // 发送消息
            Messenger.Default.Send(state, MessageToken.StatusBarState);

            Messenger.Default.Send(ResultControlVisable, MessageToken.ResultControlVisable);

            Messenger.Default.Send(CategoryId, MessageToken.ResultCategoryId);

            Messenger.Default.Send(ResultDataList, MessageToken.ResultDataList);

            if (state is 1)
            {
                await UpdateHistoryAsync(CurrentType, CurrentChannel, CurrentLink);

                await UpdateTaskbarJumpListAsync(CurrentType, CurrentChannel, CurrentLink);
            }
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
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

        /// <summary>
        /// 更新应用任务栏跳转列表内容
        /// </summary>
        private async Task UpdateTaskbarJumpListAsync(int currentType, int currentChannel, string currentLink)
        {
            if (Program.ApplicationRoot.TaskbarJumpList is not null)
            {
                int isDuplicatedIndex = -1;

                for (int index = 0; index < Program.ApplicationRoot.TaskbarJumpList.Items.Count; index++)
                {
                    JumpListItem item = Program.ApplicationRoot.TaskbarJumpList.Items[index];
                    if (item.DisplayName == currentLink)
                    {
                        isDuplicatedIndex = index;
                    }
                }

                // 无重复元素，直接添加
                if (isDuplicatedIndex is -1)
                {
                    if (HistoryRecordService.HistoryJumpListNum.HistoryJumpListNumValue is not "Unlimited")
                    {
                        int count = Convert.ToInt32(HistoryRecordService.HistoryJumpListNum.HistoryJumpListNumValue);

                        while (Program.ApplicationRoot.TaskbarJumpList.Items.Count >= count)
                        {
                            Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(0);
                        }
                    }
                    JumpListItem jumpListItem = JumpListItem.CreateWithArguments(string.Format("JumpList {0} {1} {2}", TypeList[currentType].ShortName, ChannelList[currentChannel].ShortName, currentLink), currentLink);
                    jumpListItem.GroupName = AppJumpList.GroupName;
                    jumpListItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/History.png");
                    Program.ApplicationRoot.TaskbarJumpList.Items.Add(jumpListItem);
                }
                // 有重复元素，移动到最后一位
                else
                {
                    Program.ApplicationRoot.TaskbarJumpList.Items.Add(Program.ApplicationRoot.TaskbarJumpList.Items[isDuplicatedIndex]);
                    Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(isDuplicatedIndex);
                }

                await Program.ApplicationRoot.TaskbarJumpList.SaveAsync();
            }
        }

        /// <summary>
        /// 按设置选项设置的内容过滤列表
        /// </summary>
        private void ResultListFilter(ref List<ResultModel> resultDataList)
        {
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
        /// 生成时间戳
        /// </summary>
        private long GenerateTimeStamp()
        {
            TimeSpan TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(TimeSpan.TotalSeconds);
        }
    }
}
