using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.ObjectModel;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class ResultViewModel : ObservableRecipient
    {
        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set { SetProperty(ref _resultCotnrolVisable, value); }
        }

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

        private string _resultCountInfo;

        public string ResultCountInfo
        {
            get { return _resultCountInfo; }

            set { SetProperty(ref _resultCountInfo, value); }
        }

        public ObservableCollection<ResultData> ResultDataList { get; set; } = new ObservableCollection<ResultData>();

        public ResultViewModel()
        {
            Messenger.Register<ResultViewModel, ResultControlVisableMessage>(this, (resultViewModel, resultControlVisableMessage) =>
            {
                resultViewModel.ResultControlVisable = resultControlVisableMessage.Value;
            });

            Messenger.Register<ResultViewModel, ResultCategoryIdMessage>(this, (resultViewModel, resultCategoryIdMessage) =>
            {
                resultViewModel.CategoryId = string.Format(LanguageService.GetResources("/Home/CategoryId"), resultCategoryIdMessage.Value);
            });

            // TODO:需要性能优化
            Messenger.Register<ResultViewModel, ResultDataListMessage>(this, (resultViewModel, resultDataListMessage) =>
            {
                resultViewModel.ResultDataList.Clear();
                resultViewModel.ResultCount = 0;

                for (int i = 0; i < resultDataListMessage.Value.Count; i++)
                {
                    resultDataListMessage.Value[i].SerialNumber = (i + 1).ToString();
                    resultViewModel.ResultDataList.Add(resultDataListMessage.Value[i]);
                }

                resultViewModel.ResultCount = resultViewModel.ResultDataList.Count;
                resultViewModel.ResultCountInfo = string.Format(LanguageService.GetResources("/Home/ResultCountInfo"), resultViewModel.ResultDataList.Count);
            });
        }
    }
}
