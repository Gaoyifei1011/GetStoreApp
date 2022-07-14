﻿using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class RequestControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public RequestViewModel ViewModel { get; }

        public RequestControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<RequestViewModel>();
            this.InitializeComponent();
        }
    }
}
