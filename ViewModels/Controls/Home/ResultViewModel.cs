using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        private bool _isSelectionMode = false;

        public bool IsSelectionMode
        {
            get { return _isSelectionMode; }

            set { SetProperty(ref _isSelectionMode, value); }
        }

        public IRelayCommand SelectCommand { get; set; }

        public IRelayCommand CancelCommand { get; set; }

        public ObservableCollection<ResultModel> ResultDataList { get; set; } = new ObservableCollection<ResultModel>();

        public ResultViewModel()
        {
            SelectCommand = new RelayCommand(() => { IsSelectionMode = true; });

            CancelCommand = new RelayCommand(() => { IsSelectionMode = false; });

            Messenger.Register<ResultViewModel, ResultControlVisableMessage>(this, (resultViewModel, resultControlVisableMessage) =>
            {
                resultViewModel.ResultControlVisable = resultControlVisableMessage.Value;
            });

            Messenger.Register<ResultViewModel, ResultCategoryIdMessage>(this, (resultViewModel, resultCategoryIdMessage) =>
            {
                resultViewModel.CategoryId = string.Format(LanguageService.GetResources("/Home/CategoryId"), resultCategoryIdMessage.Value);
            });

            Messenger.Register<ResultViewModel, ResultDataListMessage>(this, async (resultViewModel, resultDataListMessage) =>
            {
                resultViewModel.ResultDataList.Clear();

                for (int i = 0; i < resultDataListMessage.Value.Count; i++)
                {
                    resultDataListMessage.Value[i].SerialNumber = (i + 1).ToString();
                    resultViewModel.ResultDataList.Add(resultDataListMessage.Value[i]);
                }
                await Task.CompletedTask;
            });
        }
    }
}
