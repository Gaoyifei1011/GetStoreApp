using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
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
        public ObservableCollection<ResultModel> ResultDataList { get; } = new ObservableCollection<ResultModel>();

        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set { SetProperty(ref _resultCotnrolVisable, value); }
        }

        private string _categoryId;

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

        public IAsyncRelayCommand CopyIDCommand => new AsyncRelayCommand(async () =>
        {
            CopyPasteHelper.CopyToClipBoard(CategoryId);
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand CopyContentCommand => new AsyncRelayCommand<ResultModel>(async (param) =>
        {
            await CopyContentAsync(param);
        });

        public IAsyncRelayCommand CopyLinkCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            await CopyLinkAsync(param);
        });

        public IAsyncRelayCommand DownloadCommand { get; }

        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            await SelectNoneAsync();
            IsSelectMode = true;
        });

        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = true;
            }

            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(SelectNoneAsync);

        public IAsyncRelayCommand CopySelectedCommand => new AsyncRelayCommand(CopySelectedAsync);

        public IAsyncRelayCommand CopySelectedLinkCommand => new AsyncRelayCommand(CopySelectedLinkAsync);

        public IAsyncRelayCommand DownloadSelectedCommand { get; }

        public IAsyncRelayCommand ItemClickCommand => new AsyncRelayCommand<ItemClickEventArgs>(async (param) =>
        {
            ResultModel resultItem = (ResultModel)param.ClickedItem;
            int ClickedIndex = ResultDataList.IndexOf(resultItem);

            ResultDataList[ClickedIndex].IsSelected = !ResultDataList[ClickedIndex].IsSelected;

            await Task.CompletedTask;
        });

        /// <summary>
        /// 根据设置存储的文件链接操作方式操作获取到的文件链接
        /// </summary>
        public IAsyncRelayCommand FileOperationCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(param));
        });

        public ResultViewModel()
        {
            WeakReferenceMessenger.Default.Register<ResultViewModel, ResultControlVisableMessage>(this, (resultViewModel, resultControlVisableMessage) =>
            {
                resultViewModel.ResultControlVisable = resultControlVisableMessage.Value;
            });

            WeakReferenceMessenger.Default.Register<ResultViewModel, ResultCategoryIdMessage>(this, (resultViewModel, resultCategoryIdMessage) =>
            {
                resultViewModel.CategoryId = resultCategoryIdMessage.Value;
            });

            WeakReferenceMessenger.Default.Register<ResultViewModel, ResultDataListMessage>(this, async (resultViewModel, resultDataListMessage) =>
            {
                resultViewModel.ResultDataList.Clear();

                foreach (ResultModel resultItem in resultDataListMessage.Value)
                {
                    resultItem.IsSelected = false;
                    resultViewModel.ResultDataList.Add(resultItem);
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 单选模式下复制选中行的链接
        /// <summary>
        private async Task CopyLinkAsync(string fileLink)
        {
            CopyPasteHelper.CopyToClipBoard(fileLink);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 单选模式下复制选中行的信息
        /// <summary>
        private async Task CopyContentAsync(ResultModel resultItem)
        {
            string CopyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                resultItem.FileName,
                resultItem.FileLink,
                resultItem.FileSHA1,
                resultItem.FileSize
                );

            CopyPasteHelper.CopyToClipBoard(CopyContent);
            await Task.CompletedTask;
        }

        private async Task SelectNoneAsync()
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = false;
            }
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

            foreach (ResultModel resultItem in SelectedResultDataList)
            {
                stringBuilder.Append(string.Format("{0}\n", resultItem.FileLink));
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

            foreach (ResultModel resultItem in SelectedResultDataList)
            {
                stringBuilder.Append(string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                    resultItem.FileName,
                    resultItem.FileLink,
                    resultItem.FileSHA1,
                    resultItem.FileSize)
                    );
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            await Task.CompletedTask;
        }
    }
}
