using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class ResultViewModel : ObservableRecipient
    {
        // 服务器是否返回了结果
        private bool _isResultExist = false;

        public bool IsResultExist
        {
            get { return _isResultExist; }

            set { SetProperty(ref _isResultExist, value); }
        }

        // 成功生成链接后应用包的CateGoryID值
        private string _categoryId = string.Empty;

        public string CategoryId
        {
            get { return _categoryId; }

            set { SetProperty(ref _categoryId, value); }
        }

        private int _resultCount = 0;

        public int ResultCount
        {
            get { return _resultCount; }

            set { SetProperty(ref _resultCount, value); }
        }

        // 成功生成链接后的数量值显示结果
        private string _resultCountInfo;

        public string ResultCountInfo
        {
            get { return _resultCountInfo; }

            set { SetProperty(ref _resultCountInfo, value); }
        }

        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        // 保存获取结果数据列表
        public ObservableCollection<ResultDataModel> ResultDataList = new ObservableCollection<ResultDataModel>();

        // 结果数据列表，过滤后的结果
        public ObservableCollection<ResultDataModel> ResultDataWithFilterList = new ObservableCollection<ResultDataModel>();

        public ResultViewModel()
        {
            // 设置控件的显示状态
            Messenger.Register<ResultViewModel, ResultControlVisableMessage>(this, (resultViewModel, resultControlVisableMessage) =>
            {
                resultViewModel.IsResultExist = resultControlVisableMessage.Value;
            });

            // 添加CategoryId信息
            Messenger.Register<ResultViewModel, ResultCategoryIdMessage>(this, (resultViewModel, resultCategoryIdMessage) =>
            {
                resultViewModel.CategoryId = string.Format(HomeViewModel.CategoryId, resultCategoryIdMessage.Value);
            });

            // TODO:需要性能优化
            // 解析获取到的数据，并计算获取到的条目数量
            Messenger.Register<ResultViewModel, ResultDataListMessage>(this, (resultViewModel, resultDataListMessage) =>
            {
                // 清空上一次获取的数据内容
                resultViewModel.ResultDataList.Clear();
                resultViewModel.ResultCount = 0;

                for (int i = 0; i < resultDataListMessage.Value.Count; i++)
                {
                    // 添加序号
                    resultDataListMessage.Value[i].Index = (i + 1).ToString();
                    resultViewModel.ResultDataList.Add(resultDataListMessage.Value[i]);
                }

                resultViewModel.ResultCount = resultViewModel.ResultDataList.Count;
                resultViewModel.ResultCountInfo = string.Format(HomeViewModel.ResultCountInfo, resultViewModel.ResultDataList.Count);
            });
        }
    }
}
