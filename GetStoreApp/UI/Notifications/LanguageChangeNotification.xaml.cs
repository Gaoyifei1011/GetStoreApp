using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class LanguageChangeNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public LanguageChangeViewModel ViewModel { get; } = IOCHelper.GetService<LanguageChangeViewModel>();

        public LanguageChangeNotification(object[] notification)
        {
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]));
        }
    }
}
