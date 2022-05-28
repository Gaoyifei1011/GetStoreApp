using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Behaviors;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Home
{
    /// <summary>
    /// 该类主要用于获取链接数据，并在获取链接时修改StatusBar的状态显示和结果控件的内容
    /// </summary>
    public class GetLinksService : ObservableRecipient, IDisposable
    {
        // 选择应用的地区
        private string RegionCodeName { get; set; } = RegionService.RegionCodeName;

        public GetLinksService()
        {
            Messenger.Register<GetLinksService, RegionMessage>(this, (getLinsService, regionMessage) =>
            {
                getLinsService.RegionCodeName = regionMessage.Value;
            });
        }

        // 在对象被回收时关闭所有消息接收
        public void Dispose()
        {
            Messenger.UnregisterAll(this);
        }

        // 设置获取数据时的相关控件状态
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

        // 获得网页数据
        public async Task<HttpRequestDataModel> RequestDataAsync(string selectedType, string url, string selectedChannel)
        {
            // 获取数据
            HtmlRequestService htmlRequestService = new HtmlRequestService();

            // 设置获取数据时的相关控件状态
            SetGettingLinksState();

            // 生成请求的内容
            string content = htmlRequestService.GenerateContent(selectedType, url, selectedChannel, RegionCodeName);

            // 通过Post请求获得网页中的数据
            return await htmlRequestService.HttpRequestAsync(content);
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="httpRequestData"></param>
        public void ParseData(HttpRequestDataModel httpRequestData)
        {
            // 正常返回，解析标签内容
            if (httpRequestData.RequestId == 0)
            {
                using HtmlParseService htmlParseService = new HtmlParseService(httpRequestData);

                bool RequestState = htmlParseService.IsSuccessfulRequest();

                // 返回成功的结果，显示结果控件
                Messenger.Send(new ResultControlVisableMessage(true));

                // 服务器返回有链接的数据
                if (RequestState)
                {
                    // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                    SetGetLinksFinishedState(StateImageMode.Success);

                    // 解析成功获取到的CateGoryId信息
                    Messenger.Send(new ResultCategoryIdMessage(htmlParseService.HtmlParseCID()));

                    htmlParseService.HtmlParseLinks();
                    // 解析获取到的链接数据列表
                    Messenger.Send(new ResultDataListMessage(htmlParseService.HtmlParseLinks()));
                }

                // 服务器没有返回带有链接的数据
                else
                {
                    Messenger.Send(new ResultDataListMessage(new ObservableCollection<ResultDataModel>()));
                    // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                    SetGetLinksFinishedState(StateImageMode.Warning);
                }
            }

            // 从服务器获取数据的时候发生了错误
            else
            {
                // 返回失败的结果，不显示结果控件
                Messenger.Send(new ResultControlVisableMessage(false));

                // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                SetGetLinksFinishedState(StateImageMode.Error);

                //TODO:显示异常信息按钮，在详细信息按钮中查询具体异常
            }
        }
    }
}