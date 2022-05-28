using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services;
using GetStoreApp.Messages;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class TitleViewModel : ObservableRecipient
    {
        private readonly INavigationService _navigationService;

        // Main_Status_Instruction按钮的显示状态
        private bool _useInsVisValue = UseInstructionService.UseInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        // 主页面“使用说明”按钮的ICommand实现
        private ICommand _useInstructionCommand;

        public ICommand UseInstructionCommand
        {
            get { return _useInstructionCommand; }

            set { SetProperty(ref _useInstructionCommand, value); }
        }

        public TitleViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            // 响应“使用说明”按钮的点击事件
            UseInstructionCommand = new RelayCommand(UseIns_Clicked);

            // 设置按钮中的选项发生更改时，主页面对应的控件立即显示修改后的状态（不需要重新启动）
            Messenger.Register<TitleViewModel, UseInstructionMessage>(this, (titleViewModel, useInstructionMessage) => titleViewModel.UseInsVisValue = useInstructionMessage.Value);
        }

        //点击“使用说明”按钮后切换到关于页面查看详细信息
        private void UseIns_Clicked()
        {
            _navigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
        }
    }
}