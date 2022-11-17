using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Controls.Download;
using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Contracts.Controls.Settings.Experiment;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using GetStoreAppCore.Data;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class ResultViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object ResultDataListLock = new object();

        private IDownloadDBService DownloadDBService { get; } = ContainerHelper.GetInstance<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = ContainerHelper.GetInstance<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = ContainerHelper.GetInstance<IDownloadOptionsService>();

        private INetWorkMonitorService NetWorkMonitorService { get; } = ContainerHelper.GetInstance<INetWorkMonitorService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

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

        // 复制CategoryID
        public IRelayCommand CopyIDCommand => new RelayCommand(() =>
        {
            CopyPasteHelper.CopyToClipBoard(CategoryId);
            new ResultIDCopyNotification(true).Show();
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (ResultDataListLock)
            {
                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
        });

        // 全选
        public IRelayCommand SelectAllCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (ResultDataListLock)
            {
                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelected = true;
                }
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(() =>
        {
            // 保证线程安全
            lock (ResultDataListLock)
            {
                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelected = false;
                }
            }
        });

        // 复制选定项目的内容
        public IRelayCommand CopySelectedCommand => new RelayCommand(async () =>
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
                stringBuilder.AppendLine(string.Format("[\n{0}\n{1}\n{2}\n{3}\n]",
                    resultItem.FileName,
                    resultItem.FileLink,
                    resultItem.FileSHA1,
                    resultItem.FileSize)
                    );
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultContentCopyNotification(true, true, SelectedResultDataList.Count).Show();
        });

        // 复制选定项目的链接
        public IRelayCommand CopySelectedLinkCommand => new RelayCommand(async () =>
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
                stringBuilder.AppendLine(string.Format("{0}", resultItem.FileLink));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultLinkCopyNotification(true, true, SelectedResultDataList.Count).Show();
        });

        // 下载选定项目
        public IRelayCommand DownloadSelectedCommand => new RelayCommand(async () =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected == true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                List<BackgroundModel> duplicatedList = new List<BackgroundModel>();

                bool IsDownloadSuccessfully = false;

                foreach (ResultModel resultItem in SelectedResultDataList)
                {
                    string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                    BackgroundModel backgroundItem = new BackgroundModel
                    {
                        DownloadKey = UniqueKeyHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                        FileName = resultItem.FileName,
                        FileLink = resultItem.FileLink,
                        FilePath = DownloadFilePath,
                        TotalSize = 0,
                        FileSHA1 = resultItem.FileSHA1,
                        DownloadFlag = 1
                    };

                    DuplicatedDataInfoArgs CheckResult = await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                    if (CheckResult == DuplicatedDataInfoArgs.None)
                    {
                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                        IsDownloadSuccessfully = true;
                    }
                    else
                    {
                        duplicatedList.Add(backgroundItem);
                    }
                }

                if (duplicatedList.Count > 0)
                {
                    if (!App.IsDialogOpening)
                    {
                        App.IsDialogOpening = true;

                        ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.MultiRecord).ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            foreach (BackgroundModel backgroundItem in duplicatedList)
                            {
                                try
                                {
                                    if (File.Exists(backgroundItem.FilePath))
                                    {
                                        File.Delete(backgroundItem.FilePath);
                                    }
                                }
                                finally
                                {
                                    await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                    IsDownloadSuccessfully = true;
                                }
                            }
                        }
                        else if (result == ContentDialogResult.Secondary)
                        {
                            NavigationService.NavigateTo(typeof(DownloadPage));
                        }
                        App.IsDialogOpening = false;
                    }
                }

                // 显示下载任务创建成功消息
                new DownloadCreateNotification(IsDownloadSuccessfully).Show();
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                foreach (ResultModel resultItem in SelectedResultDataList)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
                }
            }
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 在多选模式下点击项目选择相应的条目
        public IRelayCommand ItemClickCommand => new RelayCommand<ItemClickEventArgs>((args) =>
        {
            ResultModel resultItem = (ResultModel)args.ClickedItem;
            int ClickedIndex = ResultDataList.IndexOf(resultItem);

            lock (ResultDataListLock)
            {
                ResultDataList[ClickedIndex].IsSelected = !ResultDataList[ClickedIndex].IsSelected;
            }
        });

        // 根据设置存储的文件链接操作方式操作获取到的文件链接
        public IRelayCommand DownloadCommand => new RelayCommand<ResultModel>(async (resultItem) =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                BackgroundModel backgroundItem = new BackgroundModel
                {
                    DownloadKey = UniqueKeyHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                    FileName = resultItem.FileName,
                    FileLink = resultItem.FileLink,
                    FilePath = DownloadFilePath,
                    TotalSize = 0,
                    FileSHA1 = resultItem.FileSHA1,
                    DownloadFlag = 1
                };

                // 检查是否存在相同的任务记录
                DuplicatedDataInfoArgs CheckResult = await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                switch (CheckResult)
                {
                    case DuplicatedDataInfoArgs.None:
                        {
                            await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                            new DownloadCreateNotification(true).Show();
                            break;
                        }

                    case DuplicatedDataInfoArgs.Unfinished:
                        {
                            if (!App.IsDialogOpening)
                            {
                                App.IsDialogOpening = true;

                                ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Unfinished).ShowAsync();

                                if (result == ContentDialogResult.Primary)
                                {
                                    try
                                    {
                                        if (File.Exists(backgroundItem.FilePath))
                                        {
                                            File.Delete(backgroundItem.FilePath);
                                        }
                                    }
                                    finally
                                    {
                                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                        new DownloadCreateNotification(true).Show();
                                    }
                                }
                                else if (result == ContentDialogResult.Secondary)
                                {
                                    NavigationService.NavigateTo(typeof(DownloadPage));
                                }
                                App.IsDialogOpening = false;
                            }
                            break;
                        }

                    case DuplicatedDataInfoArgs.Completed:
                        {
                            if (!App.IsDialogOpening)
                            {
                                App.IsDialogOpening = true;

                                ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Completed).ShowAsync();

                                if (result == ContentDialogResult.Primary)
                                {
                                    try
                                    {
                                        if (File.Exists(backgroundItem.FilePath))
                                        {
                                            File.Delete(backgroundItem.FilePath);
                                        }
                                    }
                                    finally
                                    {
                                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                        new DownloadCreateNotification(true).Show();
                                    }
                                }
                                else if (result == ContentDialogResult.Secondary)
                                {
                                    NavigationService.NavigateTo(typeof(DownloadPage));
                                }
                                App.IsDialogOpening = false;
                            }
                            break;
                        }
                }
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
            }
        });

        // 复制指定项目的链接
        public IRelayCommand CopyLinkCommand => new RelayCommand<string>((fileLink) =>
        {
            CopyPasteHelper.CopyToClipBoard(fileLink);

            new ResultLinkCopyNotification(true, false).Show();
        });

        // 复制指定项目的内容
        public IRelayCommand CopyContentCommand => new RelayCommand<ResultModel>((copyContent) =>
        {
            string CopyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                copyContent.FileName,
                copyContent.FileLink,
                copyContent.FileSHA1,
                copyContent.FileSize
                );

            CopyPasteHelper.CopyToClipBoard(CopyContent);

            new ResultContentCopyNotification(true, false).Show();
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

            WeakReferenceMessenger.Default.Register<ResultViewModel, ResultDataListMessage>(this, (resultViewModel, resultDataListMessage) =>
            {
                lock (ResultDataListLock)
                {
                    resultViewModel.ResultDataList.Clear();
                }

                lock (ResultDataListLock)
                {
                    foreach (ResultData resultItem in resultDataListMessage.Value)
                    {
                        resultViewModel.ResultDataList.Add(new ResultModel
                        {
                            IsSelected = false,
                            FileName = resultItem.FileName,
                            FileLink = resultItem.FileLink,
                            FileLinkExpireTime = resultItem.FileLinkExpireTime,
                            FileSHA1 = resultItem.FileSHA1,
                            FileSize = resultItem.FileSize
                        });
                    }
                }
            });

            WeakReferenceMessenger.Default.Register<ResultViewModel, WindowClosedMessage>(this, (resultViewModel, windowClosedMessage) =>
            {
                if (windowClosedMessage.Value)
                {
                    WeakReferenceMessenger.Default.UnregisterAll(this);
                }
            });
        }
    }
}
