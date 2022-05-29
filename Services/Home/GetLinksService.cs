using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Behaviors;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// TODO:代码优化
namespace GetStoreApp.Services.Home
{
    /// <summary>
    /// 获取链接服务（包括获取数据并解析数据，修改StatusBar的显示状态）
    /// Get linked services (including getting data and parsing data, modifying status of StatusBar)
    /// </summary>
    public class GetLinksService : ObservableRecipient, IDisposable
    {
        /// <summary>
        /// 从API获取信息时对应的国家/地区
        /// The country/region that corresponds to the information obtained from the API
        /// </summary>
        private string RegionCodeName { get; set; } = RegionService.RegionCodeName;

        // 是否要过滤扩展名以“e”开头的文件
        private bool StartsWithEFilterValue { get; set; } = LinkFilterService.LinkFilterValue[0];

        // 是否要过滤扩展名以“blockmap”开头的文件
        private bool BlockMapFilterValue { get; set; } = LinkFilterService.LinkFilterValue[1];


        // 返回失败的结果，不显示结果控件
        private List<ResultDataModel> ResultDataList = new List<ResultDataModel>();

        public GetLinksService()
        {
            // 类在初始化时注册消息接收
            // The class registers for message reception when initialized
            Messenger.Register<GetLinksService, RegionMessage>(this, (getLinsService, regionMessage) =>
            {
                getLinsService.RegionCodeName = regionMessage.Value;
            });

            // 注册过滤状态消息
            Messenger.Register<GetLinksService, StartsWithEFilterMessage>(this, (getLinksService, startsWithEFilterMessage) =>
            {
                getLinksService.StartsWithEFilterValue = startsWithEFilterMessage.Value;
            });

            Messenger.Register<GetLinksService, BlockMapFilterService>(this, (getLinksService, blockMapFilterService) =>
            {
                getLinksService.BlockMapFilterValue = blockMapFilterService.Value;
            });
        }

        /// <summary>
        /// 类在结束使用后准备回收时注销消息接收
        /// The class receives an unregister message when it is ready for recycling after it is finished in use
        /// </summary>
        public void Dispose()
        {
            Messenger.UnregisterAll(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 向API发送请求，解析数据时，将StatusBar控件状态改成正在获取中
        /// When sending a request to the API to parse the data, change the StatusBar control state to Being fetched
        /// </summary>
        private void SetGettingLinksState()
        {
            // 设置StatusImage图标：提示状态
            Messenger.Send(new StateImageModeMessage(StateImageMode.Notification));

            // 点击按钮获取网页信息后，设置StatusInfo文本：正在获取中
            Messenger.Send(new StateInfoTextMessage(HomeViewModel.StatusInfoGetting));

            // 显示StatusProgressring圆环动画
            Messenger.Send(new StatePrRingVisValueMessage(true));

            // StatusProgressring圆环动画未激活时，激活圆环
            Messenger.Send(new StatePrRingActValueMessage(true));
        }

        /// <summary>
        /// 解析数据完成后，将StatusBar控件状态改成获取完成后对应的各种信息
        /// After the data is parsed, change the status of the StatusBar control to the corresponding information after obtaining it
        /// </summary>
        /// <param name="state">传入的状态值</param>
        private void SetGetLinksFinishedState(StateImageMode state)
        {
            // 设置StateImage图标，根据传入的值来选择
            Messenger.Send(new StateImageModeMessage(state));

            switch (state)
            {
                case StateImageMode.Error:
                    {
                        // 设置StatusInfoText文本：获取失败
                        Messenger.Send(new StateInfoTextMessage(HomeViewModel.StatusInfoError));
                        break;
                    }
                case StateImageMode.Warning:
                    {
                        // 设置StatusInfoText文本：获取发生了异常，警示状态
                        Messenger.Send(new StateInfoTextMessage(HomeViewModel.StatusInfoWarning));
                        break;
                    }
                case StateImageMode.Notification:
                    {
                        break;
                    }
                case StateImageMode.Success:
                    {
                        // 设置StatusInfoText文本：获取成功
                        Messenger.Send(new StateInfoTextMessage(HomeViewModel.StatusInfoSuccess));
                        break;
                    }
                default:
                    {
                        // 设置StatusInfoText文本：获取失败
                        Messenger.Send(new StateInfoTextMessage(HomeViewModel.StatusInfoError));
                        break;
                    }
            }
            // 关闭圆环显示
            Messenger.Send(new StatePrRingActValueMessage(false));
        }

        /// <summary>
        /// 向API发送Post并从API中获得原始数据
        /// Send Post to the API and get raw data from the API
        /// </summary>
        /// <param name="selectedType">选定的类型</param>
        /// <param name="url">输入的获取信息</param>
        /// <param name="selectedChannel">选定的通道</param>
        /// <returns>获取到的原始数据</returns>
        public async Task<HttpRequestDataModel> RequestDataAsync(string selectedType, string url, string selectedChannel)
        {
            // 创建获取服务
            HtmlRequestService htmlRequestService = new HtmlRequestService();

            // 设置获取数据时的相关控件状态
            SetGettingLinksState();

            // 生成请求的内容
            string content = HtmlRequestService.GenerateContent(selectedType, url, selectedChannel, RegionCodeName);

            // 通过Post请求获得网页中的数据
            return await htmlRequestService.HttpRequestAsync(content);
        }

        /// <summary>
        /// 解析获取到的数据
        /// Parse the obtained data
        /// </summary>
        /// <param name="httpRequestData">Post请求中获取到的原始数据</param>
        public void ParseData(HttpRequestDataModel httpRequestData)
        {
            bool ResultControlVisable;

            string CategoryId = string.Empty;

            // 正常返回，解析标签内容
            if (httpRequestData.RequestId == 0)
            {
                // 创建解析服务
                using HtmlParseService htmlParseService = new HtmlParseService(httpRequestData);

                ResultControlVisable = true;

                // 查看是否从API返回带有内容的数据
                bool RequestState = htmlParseService.IsSuccessfulRequest();

                // API在成功请求后没有返回带有链接的数据
                if (RequestState)
                {
                    // 设置StatusBar的控件状态为警告
                    SetGetLinksFinishedState(StateImageMode.Warning);
                }

                // API在成功请求后返回带有链接的数据
                else
                {
                    // 设置StatusBar的控件状态为成功
                    SetGetLinksFinishedState(StateImageMode.Success);

                    // 解析成功获取到的CateGoryId信息
                    CategoryId = htmlParseService.HtmlParseCID();

                    ResultDataList = htmlParseService.HtmlParseLinks();
                }
            }

            // 从服务器获取数据的时候发生了错误
            else
            {
                // 设置StatusBar的控件状态为错误
                SetGetLinksFinishedState(StateImageMode.Error);

                // 不显示结果控件
                ResultControlVisable = false;

                //TODO:显示异常信息按钮，在详细信息按钮中可以看到异常内容


            }

            // 过滤
            if (StartsWithEFilterValue)
            {
                ResultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (BlockMapFilterValue)
            {
                ResultDataList.RemoveAll(item=>item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }

            // 发送消息
            Messenger.Send(new ResultControlVisableMessage(ResultControlVisable));

            Messenger.Send(new ResultCategoryIdMessage(CategoryId));

            Messenger.Send(new ResultDataListMessage(ResultDataList));
        }
    }
}
