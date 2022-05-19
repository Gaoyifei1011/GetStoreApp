using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

namespace GetStoreApp.ViewModels.Controls.Main
{
    public class ResultViewModel : ObservableObject
    {
        // 服务器是否返回了正确的结果
        private bool _mainResult = false;

        public bool MainResult
        {
            get { return _mainResult; }

            set { SetProperty(ref _mainResult, value); }
        }

        // 成功生成链接后应用包的CateGoryID值
        private string _mainCategoryId = string.Empty;

        public string MainCategoryId
        {
            get { return _mainCategoryId; }

            set { SetProperty(ref _mainCategoryId, value); }
        }

        // 成功生成链接后的数量值显示结果
        private string _mainResultCount = string.Empty;

        public string MainResultCount
        {
            get { return _mainResultCount; }

            set { SetProperty(ref _mainResultCount, value); }
        }

        // 初始化结果数据列表
        public ObservableCollection<ResultDataModel> ResultDataList = new ObservableCollection<ResultDataModel>();

        public ResultViewModel()
        {
            // 设置控件的显示状态并清空列表
            Messenger.Default.Register(this, "MainResult", (bool obj) => { MainResult = obj; ResultDataList.Clear(); });

            // 清空数据列表
            //Messenger.Default.Register(this, "ResultDataListClear", (bool obj) => { if (obj) { ResultDataList.Clear(); } });

            // 添加CategoryId信息
            Messenger.Default.Register(this, "MainCategoryId", (string obj) => { MainCategoryId = string.Concat(LanguageSelectorService.GetResources("Main_Result_CategoryID"), obj); });

            // 解析获取到的数据
            Messenger.Default.Register(this, "ResultDataList", (ObservableCollection<ResultDataModel> obj) =>
            {
                foreach (var item in obj)
                {
                    ResultDataList.Add(item);
                }
            });

            // 结果列表中获取到的条目数量
            Messenger.Default.Register(this, "MainResultCount", (bool obj) => { if (obj) { MainResultCount = string.Format(LanguageSelectorService.GetResources("Main_Result_Count"), ResultDataList.Count); } });
        }
    }
}
