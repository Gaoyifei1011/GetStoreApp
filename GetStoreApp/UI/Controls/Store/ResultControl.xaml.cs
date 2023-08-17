using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.System;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：请求结果控件
    /// </summary>
    public sealed partial class ResultControl : Grid, INotifyPropertyChanged
    {
        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set
            {
                _resultCotnrolVisable = value;
                OnPropertyChanged();
            }
        }

        private string _categoryId;

        public string CategoryId
        {
            get { return _categoryId; }

            set
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        // 根据设置存储的文件链接操作方式操作获取到的文件链接
        public XamlUICommand DownloadCommand { get; } = new XamlUICommand();

        // 复制指定项目的链接
        public XamlUICommand CopyLinkCommand { get; } = new XamlUICommand();

        // 复制指定项目的内容
        public XamlUICommand CopyContentCommand { get; } = new XamlUICommand();

        public ObservableCollection<ResultModel> ResultDataList { get; } = new ObservableCollection<ResultModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ResultControl()
        {
            InitializeComponent();

            DownloadCommand.ExecuteRequested += async (sender, args) =>
            {
                ResultModel resultItem = args.Parameter as ResultModel;
                if (resultItem is not null)
                {
                    // 查看是否开启了网络监控服务
                    if (NetWorkMonitorService.NetWorkMonitorValue)
                    {
                        // 网络处于未连接状态，不再进行下载，显示通知
                        if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
                        {
                            if (!checkFailed)
                            {
                                new NetWorkErrorNotification(this).Show();
                                return;
                            }
                        }
                    }

                    // 使用应用内提供的下载方式
                    if (DownloadOptionsService.DownloadMode.SelectedValue == DownloadOptionsService.DownloadModeList[0].SelectedValue)
                    {
                        string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                        BackgroundModel backgroundItem = new BackgroundModel
                        {
                            DownloadKey = HashAlgorithmHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                            FileName = resultItem.FileName,
                            FileLink = resultItem.FileLink,
                            FilePath = DownloadFilePath,
                            TotalSize = 0,
                            FileSHA1 = resultItem.FileSHA1,
                            DownloadFlag = 1
                        };

                        // 检查是否存在相同的任务记录
                        DuplicatedDataInfoArgs CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                        switch (CheckResult)
                        {
                            case DuplicatedDataInfoArgs.None:
                                {
                                    bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                                    new DownloadCreateNotification(this, AddResult).Show();
                                    break;
                                }

                            case DuplicatedDataInfoArgs.Unfinished:
                                {
                                    ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataInfoArgs.Unfinished), this);

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LogType.WARNING, "Delete duplicated unfinished downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            new DownloadCreateNotification(this, AddResult).Show();
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadPage));
                                    }
                                    break;
                                }

                            case DuplicatedDataInfoArgs.Completed:
                                {
                                    ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataInfoArgs.Completed), this);

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LogType.WARNING, "Delete duplicated completed downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            new DownloadCreateNotification(this, AddResult).Show();
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadPage));
                                    }
                                    break;
                                }
                        }
                    }

                    // 使用浏览器下载
                    else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
                    {
                        await Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
                    }
                }
            };

            CopyLinkCommand.ExecuteRequested += (sender, args) =>
            {
                string fileLink = args.Parameter as string;

                if (fileLink is not null)
                {
                    CopyPasteHelper.CopyToClipBoard(fileLink);

                    new ResultLinkCopyNotification(this, false).Show();
                }
            };

            CopyContentCommand.ExecuteRequested += (sender, args) =>
            {
                ResultModel resultItem = args.Parameter as ResultModel;
                if (resultItem is not null)
                {
                    string copyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                        resultItem.FileName,
                        resultItem.FileLink,
                        resultItem.FileSHA1,
                        resultItem.FileSize
                        );

                    CopyPasteHelper.CopyToClipBoard(copyContent);
                    new ResultContentCopyNotification(this, false).Show();
                }
            };
        }

        /// <summary>
        /// 本地化CategoryId信息
        /// </summary>
        public string LocalizeCategoryId(string categoryId)
        {
            return string.Format(ResourceService.GetLocalized("Store/categoryId"), categoryId);
        }

        /// <summary>
        /// 本地化获取结果数量统计信息
        /// </summary>
        public string LocalizeResultCountInfo(int count)
        {
            return string.Format(ResourceService.GetLocalized("Store/ResultCountInfo"), count);
        }

        /// <summary>
        /// 复制CategoryID
        /// </summary>
        public void OnCopyIDClicked(object sender, RoutedEventArgs args)
        {
            CopyPasteHelper.CopyToClipBoard(CategoryId);
            new ResultIDCopyNotification(this).Show();
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        public void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelectMode = true;
                resultItem.IsSelected = false;
            }

            IsSelectMode = true;
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        public void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 显示复制选项
        /// </summary>
        public void OnCopyOptionsClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as MenuFlyoutItem);
        }

        /// <summary>
        /// 复制选定项目的内容
        /// </summary>
        public async void OnCopySelectedClicked(object sender, RoutedEventArgs args)
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            SelectedResultDataList.ForEach(resultItem =>
            {
                stringBuilder.AppendLine(string.Format("[\n{0}\n{1}\n{2}\n{3}\n]",
                    resultItem.FileName,
                    resultItem.FileLink,
                    resultItem.FileSHA1,
                    resultItem.FileSize)
                    );
            });

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultContentCopyNotification(this, true, SelectedResultDataList.Count).Show();
        }

        /// <summary>
        /// 复制选定项目的链接
        /// </summary>
        public async void OnCopySelectedLinkClicked(object sender, RoutedEventArgs args)
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            SelectedResultDataList.ForEach(resultItem => { stringBuilder.AppendLine(string.Format("{0}", resultItem.FileLink)); });

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultLinkCopyNotification(this, true, SelectedResultDataList.Count).Show();
        }

        /// <summary>
        /// 下载选定项目
        /// </summary>
        public async void OnDownloadSelectedClicked(object sender, RoutedEventArgs args)
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                // 网络处于未连接状态，不再进行下载，显示通知
                if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
                {
                    if (!checkFailed)
                    {
                        new NetWorkErrorNotification(this).Show();
                        return;
                    }
                }
            }

            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            };

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.SelectedValue == DownloadOptionsService.DownloadModeList[0].SelectedValue)
            {
                List<BackgroundModel> duplicatedList = new List<BackgroundModel>();

                bool IsDownloadSuccessfully = false;

                SelectedResultDataList.ForEach(async resultItem =>
                {
                    string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                    BackgroundModel backgroundItem = new BackgroundModel
                    {
                        DownloadKey = HashAlgorithmHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                        FileName = resultItem.FileName,
                        FileLink = resultItem.FileLink,
                        FilePath = DownloadFilePath,
                        TotalSize = 0,
                        FileSHA1 = resultItem.FileSHA1,
                        DownloadFlag = 1
                    };

                    DuplicatedDataInfoArgs CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                    if (CheckResult is DuplicatedDataInfoArgs.None)
                    {
                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                        IsDownloadSuccessfully = true;
                    }
                    else
                    {
                        duplicatedList.Add(backgroundItem);
                    }
                });

                if (duplicatedList.Count > 0)
                {
                    ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataInfoArgs.MultiRecord), this);

                    if (result is ContentDialogResult.Primary)
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
                            catch (Exception e)
                            {
                                LogService.WriteLog(LogType.WARNING, "Delete duplicated downloaded file failed.", e);
                            }
                            finally
                            {
                                await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                IsDownloadSuccessfully = true;
                            }
                        }
                    }
                    else if (result is ContentDialogResult.Secondary)
                    {
                        NavigationService.NavigateTo(typeof(DownloadPage));
                    }
                }

                // 显示下载任务创建成功消息
                new DownloadCreateNotification(this, IsDownloadSuccessfully).Show();

                foreach (ResultModel resultItem in ResultDataList)
                {
                    resultItem.IsSelectMode = false;
                }
                IsSelectMode = false;
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                SelectedResultDataList.ForEach(async resultItem =>
                {
                    await Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
                });
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelectMode = false;
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            ResultModel resultItem = args.ClickedItem as ResultModel;

            if (resultItem is not null)
            {
                int ClickedIndex = ResultDataList.IndexOf(resultItem);
                ResultDataList[ClickedIndex].IsSelected = !ResultDataList[ClickedIndex].IsSelected;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
