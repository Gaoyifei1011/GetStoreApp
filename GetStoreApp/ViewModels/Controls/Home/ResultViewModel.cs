using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Download;
using GetStoreApp.Models.Home;
using GetStoreApp.Models.Notification;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class ResultViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object ResultDataListLock = new object();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

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
        public IAsyncRelayCommand CopyIDCommand => new AsyncRelayCommand(async () =>
        {
            CopyPasteHelper.CopyToClipBoard(CategoryId);
            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationContent = "ResultIDCopy",
                NotificationValue = new object[] { true }
            }));

            await Task.CompletedTask;
        });

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
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
            await Task.CompletedTask;
        });

        // 全选
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            // 保证线程安全
            lock (ResultDataListLock)
            {
                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            // 保证线程安全
            lock (ResultDataListLock)
            {
                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 复制选定项目的内容
        public IAsyncRelayCommand CopySelectedCommand => new AsyncRelayCommand(async () =>
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

            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationContent = "ResultContentCopy",
                NotificationValue = new object[] { true, true, SelectedResultDataList.Count }
            }));

            await Task.CompletedTask;
        });

        // 复制选定项目的链接
        public IAsyncRelayCommand CopySelectedLinkCommand => new AsyncRelayCommand(async () =>
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

            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationContent = "ResultLinkCopy",
                NotificationValue = new object[] { true, true, SelectedResultDataList.Count }
            }));

            await Task.CompletedTask;
        });

        // 下载选定项目
        public IAsyncRelayCommand DownloadSelectedCommand => new AsyncRelayCommand(async () =>
        {
            NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

            // 网络处于未连接状态，不再进行下载，显示通知
            if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
            {
                WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
                {
                    NotificationContent = "NetWorkError",
                }));
                return;
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

                foreach (ResultModel resultItem in SelectedResultDataList)
                {
                    BackgroundModel backgroundItem = new BackgroundModel
                    {
                        DownloadKey = GenerateUniqueKey(resultItem.FileName, resultItem.FileLink, resultItem.FileSHA1),
                        FileName = resultItem.FileName,
                        FileLink = resultItem.FileLink,
                        FilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName),
                        TotalSize = 0,
                        FileSHA1 = resultItem.FileSHA1,
                        DownloadFlag = 1
                    };

                    if (await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey))
                    {
                        duplicatedList.Add(backgroundItem);
                    }
                    else
                    {
                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                    }
                }

                if (duplicatedList.Count > 0)
                {
                    ContentDialogResult result = await new DownloadNotifyDialog(duplicatedList.Count).ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        foreach (BackgroundModel backgroundItem in duplicatedList)
                        {
                            await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                        }
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }
                }
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
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 在多选模式下点击项目选择相应的条目
        public IAsyncRelayCommand ItemClickCommand => new AsyncRelayCommand<ItemClickEventArgs>(async (param) =>
        {
            ResultModel resultItem = (ResultModel)param.ClickedItem;
            int ClickedIndex = ResultDataList.IndexOf(resultItem);

            lock (ResultDataListLock)
            {
                ResultDataList[ClickedIndex].IsSelected = !ResultDataList[ClickedIndex].IsSelected;
            }

            await Task.CompletedTask;
        });

        // 根据设置存储的文件链接操作方式操作获取到的文件链接
        public IAsyncRelayCommand DownloadCommand => new AsyncRelayCommand<ResultModel>(async (param) =>
        {
            NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

            // 网络处于未连接状态，不再进行下载，显示通知
            if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
            {
                WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
                {
                    NotificationContent = "NetWorkError",
                }));
                return;
            }

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                BackgroundModel backgroundItem = new BackgroundModel
                {
                    DownloadKey = GenerateUniqueKey(param.FileName, param.FileLink, param.FileSHA1),
                    FileName = param.FileName,
                    FileLink = param.FileLink,
                    FilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, param.FileName),
                    TotalSize = 0,
                    FileSHA1 = param.FileSHA1,
                    DownloadFlag = 1
                };

                // 检查是否存在相同的任务记录
                bool CheckResult = await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                if (CheckResult)
                {
                    ContentDialogResult result = await new DownloadNotifyDialog(0).ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }
                }
                else
                {
                    await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                }
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(param.FileLink));
            }
        });

        // 复制指定项目的链接
        public IAsyncRelayCommand CopyLinkCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            CopyPasteHelper.CopyToClipBoard(param);

            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationContent = "ResultLinkCopy",
                NotificationValue = new object[] { true, false }
            }));

            await Task.CompletedTask;
        });

        // 复制指定项目的内容
        public IAsyncRelayCommand CopyContentCommand => new AsyncRelayCommand<ResultModel>(async (param) =>
        {
            string CopyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                param.FileName,
                param.FileLink,
                param.FileSHA1,
                param.FileSize
                );

            CopyPasteHelper.CopyToClipBoard(CopyContent);

            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel()
            {
                NotificationContent = "ResultContentCopy",
                NotificationValue = new object[] { true, false }
            }));

            await Task.CompletedTask;
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
                lock (ResultDataListLock)
                {
                    resultViewModel.ResultDataList.Clear();
                }

                lock (ResultDataListLock)
                {
                    foreach (ResultModel resultItem in resultDataListMessage.Value)
                    {
                        resultItem.IsSelected = false;
                        resultViewModel.ResultDataList.Add(resultItem);
                    }
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 生成唯一的下载键值
        /// </summary>
        public string GenerateUniqueKey(string fileName, string fileLink, string fileSHA1)
        {
            string Content = string.Format("{0} {1} {2}", fileName, fileLink, fileSHA1);

            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Content));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder str = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++) str.Append(data[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            // 返回十六进制字符串
            return str.ToString();
        }
    }
}
