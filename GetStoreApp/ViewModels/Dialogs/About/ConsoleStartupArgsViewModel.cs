using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Dialogs.CommonDialogs.About;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    public sealed class ConsoleStartupArgsViewModel
    {
        public string SampleShort { get; } = @"GetStoreApp.exe Console ""https://www.microsoft.com/store/productId/9WZDNCRFJBMP""";

        public string SampleFull { get; } = @"GetStoreApp.exe Console -t ""pid"" -c ""rt"" ""9WZDNCRFJBMP""";

        public string SampleWithoutParameter { get; } = @"GetStoreApp.exe Console";

        public List<StartupArgsModel> ConsoleStartupArgsList { get; } = new List<StartupArgsModel>
        {
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Type") ,Argument="-t; --type",IsRequired=ResourceService.GetLocalized("Dialog/No"),ArgumentContent=@"""url"",""pid"",""pfn"",""cid"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Channel"),Argument="-c; --channel",IsRequired=ResourceService.GetLocalized("Dialog/No"),ArgumentContent=@"""wif"",""wis"",""rp"",""rt"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Link"),Argument="-l; --link",IsRequired=ResourceService.GetLocalized("Dialog/Yes"),ArgumentContent=string.Format("[{0}]",ResourceService.GetLocalized("Dialog/LinkContent")) }
        };

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            ContentDialog dialog = sender as ContentDialog;

            if (dialog is not null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(dialog);

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                    if (current is Rectangle { Name: "SmokeLayerBackground" } background)
                    {
                        background.Margin = new Thickness(0);
                        background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 重置内容对话框烟雾背景距离顶栏的间隔
        /// </summary>
        private void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
        }
    }
}
