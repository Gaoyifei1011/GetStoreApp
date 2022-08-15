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

        public IAsyncRelayCommand CopyCategoryIDCommand { get; }

        public IAsyncRelayCommand CopyContentCommand { get; }

        public IAsyncRelayCommand CopyLinkCommand { get; }

        public IAsyncRelayCommand DownloadCommand { get; }

        public IAsyncRelayCommand SelectCommand { get; }

        public IAsyncRelayCommand CancelCommand { get; }

        public IAsyncRelayCommand SelectAllCommand { get; }

        public IAsyncRelayCommand SelectNoneCommand { get; }

        public IAsyncRelayCommand CopySelectedCommand { get; }

        public IAsyncRelayCommand CopySelectedLinkCommand { get; }

        public IAsyncRelayCommand DownloadSelectedCommand { get; }

        public IAsyncRelayCommand FileOperationCommand { get; }

        public ResultViewModel()
        {
            CopyCategoryIDCommand = new AsyncRelayCommand(async () =>
            {
                CopyPasteHelper.CopyToClipBoard(CategoryId); await Task.CompletedTask;
            });

            CopyContentCommand = new AsyncRelayCommand(CopyContentAsync);

            CopyLinkCommand = new AsyncRelayCommand(CopyLinkAsync);

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

            CopySelectedLinkCommand = new AsyncRelayCommand(CopySelectedLinkAsync);

            FileOperationCommand = new AsyncRelayCommand<string>(async (param) =>
            {
                await FileOperationAsync(param);
            });

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
        /// 单选模式下复制选中行的链接
        /// <summary>
        private async Task CopyLinkAsync()
        {
            if (SelectedResultItem == null)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            CopyPasteHelper.CopyToClipBoard(SelectedResultItem.FileLink);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 单选模式下复制选中行的信息
        /// <summary>
        private async Task CopyContentAsync()
        {
            if (SelectedResultItem == null)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
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
        /// 多选模式下复制选中行的链接
        /// </summary>
        private async Task CopySelectedLinkAsync()
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected == true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in SelectedResultDataList)
            {
                stringBuilder.Append(string.Format("[\n{0}\n]\n", item.FileLink));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 多选模式下复制选中行的信息
        /// </summary>
        private async Task CopySelectedAsync()
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected == true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
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
        /// 根据获取到的链接选择相应的操作
        /// </summary>
        private async Task FileOperationAsync(string fileLink)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(fileLink));
        }
    }
}
