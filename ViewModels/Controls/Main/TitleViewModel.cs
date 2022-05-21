using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Views;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Windows.Input;

using Windows.UI.Xaml.Media.Animation;

namespace GetStoreApp.ViewModels.Controls.Main
{
    public class TitleViewModel : ObservableObject
    {
        // Main_Status_Instruction按钮的显示状态
        private bool _mainInsVisValue = UseInstructionSettings.UseInsVisValue;

        public bool MainInsVisValue
        {
            get { return _mainInsVisValue; }

            set { SetProperty(ref _mainInsVisValue, value); }
        }

        // 主页面“使用说明”按钮的ICommand实现
        private ICommand _mainStatInsCommand;

        public ICommand MainStatInsCommand
        {
            get { return _mainStatInsCommand; }

            set { SetProperty(ref _mainStatInsCommand, value); }
        }

        public TitleViewModel()
        {
            // 响应“使用说明”按钮的点击事件
            MainStatInsCommand = new RelayCommand(MainIns_Clicked);

            // 设置按钮中的选项发生更改时，主页面对应的控件立即显示修改后的状态（不需要重新启动）
            Messenger.Default.Register(this, "UseInsVisValue", (bool obj) => { MainInsVisValue = obj; });
        }

        // 点击“使用说明”按钮后切换到关于页面查看详细信息
        private void MainIns_Clicked()
        {
            NavigationService.Navigate(typeof(AboutPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
