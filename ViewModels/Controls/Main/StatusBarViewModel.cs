using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Behaviors;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Controls.Main
{
    public class StatusBarViewModel : ObservableObject
    {
        // 初始化设置Main_Status_Image图标：提示状态
        private MainStatImageMode _mainStatImage = MainStatImageMode.Notification;

        public MainStatImageMode MainStatImage
        {
            get { return _mainStatImage; }

            set { SetProperty(ref _mainStatImage, value); }
        }

        // 初始化设置Main_Status_Info文本：欢迎使用
        private string _mainStatInfoText = LanguageSelectorService.GetResources("Main_Status_Info/Welcome");

        public string MainStatInfoText
        {
            get { return _mainStatInfoText; }

            set { SetProperty(ref _mainStatInfoText, value); }
        }

        // Main_Status_Progressring的激活状态
        private bool _mainStatPrRingActValue = false;

        public bool MainStatPrRingActValue
        {
            get { return _mainStatPrRingActValue; }

            set { SetProperty(ref _mainStatPrRingActValue, value); }
        }

        // Main_Status_Progressring的显示状态
        private bool _mainStatPrRingVisValue = false;

        public bool MainStatPrRingVisValue
        {
            get { return _mainStatPrRingVisValue; }

            set { SetProperty(ref _mainStatPrRingVisValue, value); }
        }

        public StatusBarViewModel()
        {
            // Main_Stat_Image的图标，根据传入的int值转换为Enum值确定显示的图标状态
            Messenger.Default.Register(this, "MainStatImage", (int obj) =>
            {
                MainStatImage = (MainStatImageMode)obj;
            });

            // 设置Main_Status_Info文本
            Messenger.Default.Register(this, "MainStatInfoText", (string obj) =>
            {
                MainStatInfoText = LanguageSelectorService.GetResources(obj);
            });

            // 设置Main_Status_Progressring激活和显示状态
            Messenger.Default.Register(this, "MainStatPrRingActValue", (bool obj) => { MainStatPrRingActValue = obj; });

            Messenger.Default.Register(this, "MainStatPrRingVisValue", (bool obj) => { MainStatPrRingVisValue = obj; });
        }
    }
}
