using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        private ResultModel _selectedResultItem;

        public ResultModel SelectedResultItem
        {
            get { return _selectedResultItem; }

            set { SetProperty(ref _selectedResultItem, value); }
        }

        public ObservableCollection<ResultModel> ResultDataList { get; set; } = new ObservableCollection<ResultModel>();

        public IAsyncRelayCommand CopyCategoryIDCommand { get; set; }

        public IAsyncRelayCommand CopySingleCommand { get; set; }

        public IAsyncRelayCommand SelectCommand { get; set; }

        public IAsyncRelayCommand CancelCommand { get; set; }

        public IAsyncRelayCommand SelectAllCommand { get; set; }

        public IAsyncRelayCommand SelectNoneCommand { get; set; }

        public IAsyncRelayCommand CopySelectedCommand { get; set; }

        public ResultViewModel()
        {
            CopyCategoryIDCommand = new AsyncRelayCommand(async () =>
            {
                CopyPasteHelper.CopyToClipBoard(CategoryId); await Task.CompletedTask;
            });

            CopySingleCommand = new AsyncRelayCommand(CopySingleAsync);

            SelectCommand = new AsyncRelayCommand(async () =>
            {
                await SelectNoneAsync();
                IsSelectMode = true;
            });

            CancelCommand = new AsyncRelayCommand(async () =>
            {
                IsSelectMode = false;
                await Task.CompletedTask;
            });

            SelectAllCommand = new AsyncRelayCommand(async () =>
            {
                foreach (var item in ResultDataList) item.IsSelected = true;
                await Task.CompletedTask;
            });

            SelectNoneCommand = new AsyncRelayCommand(SelectNoneAsync);

            CopySelectedCommand = new AsyncRelayCommand(CopySelectedAsync);

            Messenger.Register<ResultViewModel, ResultControlVisableMessage>(this, (resultViewModel, resultControlVisableMessage) =>
            {
                resultViewModel.ResultControlVisable = resultControlVisableMessage.Value;
            });

            Messenger.Register<ResultViewModel, ResultCategoryIdMessage>(this, (resultViewModel, resultCategoryIdMessage) =>
            {
                resultViewModel.CategoryId = resultCategoryIdMessage.Value;
            });

            Messenger.Register<ResultViewModel, ResultDataListMessage>(this, async (resultViewModel, resultDataListMessage) =>
            {
                resultViewModel.ResultDataList.Clear();

                foreach (var item in resultDataListMessage.Value)
                {
                    item.IsSelected = false;
                    resultViewModel.ResultDataList.Add(item);
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 单行单选模式下复制选中的条目
        /// <summary>
        private async Task CopySingleAsync()
        {
            if (SelectedResultItem == null)
            {
                await ShowSelectEmptyPromptDialogAsync();
                return;
            };

            string CopyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n", SelectedResultItem.FileName, SelectedResultItem.FileLink, SelectedResultItem.FileSHA1, SelectedResultItem.FileSize);

            CopyPasteHelper.CopyToClipBoard(CopyContent);
            await Task.CompletedTask;
        }

        private async Task SelectNoneAsync()
        {
            foreach (var item in ResultDataList) item.IsSelected = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 多选模式下复制选中的条目
        /// </summary>
        private async Task CopySelectedAsync()
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected == true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count == 0)
            {
                await ShowSelectEmptyPromptDialogAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in SelectedResultDataList)
            {
                stringBuilder.Append(string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n", item.FileName, item.FileLink, item.FileSHA1, item.FileSize));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 选中内容为空时，显示提示对话框
        /// </summary>
        private async Task ShowSelectEmptyPromptDialogAsync()
        {
            SelectEmptyPromptDialog dialog = new SelectEmptyPromptDialog { XamlRoot = App.MainWindow.Content.XamlRoot };
            await dialog.ShowAsync();
        }
    }
}
