using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 微软商店页面
    /// </summary>
    public sealed partial class StorePage : Page, INotifyPropertyChanged
    {
        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StorePage()
        {
            InitializeComponent();
            StoreSelectorBar.SelectedItem = StoreSelectorBar.Items[SelectedIndex];
        }

        #region 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 分割控件选中项发生改变时引发的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender is SelectorBar selectorBar && selectorBar.SelectedItem is not null)
            {
                SelectedIndex = selectorBar.Items.IndexOf(selectorBar.SelectedItem);
            }
        }

        private void OnFlipViewSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is FlipView flipView && flipView.SelectedItem is not null)
            {
                SelectedIndex = flipView.Items.IndexOf(flipView.SelectedItem);
            }
        }

        /// <summary>
        /// 打开设置中的语言和区域
        /// </summary>
        private void OnLanguageAndRegionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.Instructions);
        }

        private void OnFlipViewLoaded(object sender, RoutedEventArgs args)
        {
            if (VisualTreeHelper.GetChildrenCount(sender as FlipView) > 0)
            {
                FrameworkElement layoutRoot = (FrameworkElement)VisualTreeHelper.GetChild(sender as FlipView, 0);

                layoutRoot.PointerWheelChanged -= OnPointerWheelChanged;
                layoutRoot.PointerWheelChanged += OnPointerWheelChanged;
            }
        }

        private static void OnPointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                if (VisualTreeHelper.GetParent(frameworkElement) is FlipView flipView)
                {
                    args.Handled = true;
                }
                else
                {
                    frameworkElement.PointerWheelChanged -= OnPointerWheelChanged;
                }
            }
        }

        #endregion 第二部分：应用商店页面——挂载的事件
    }
}
