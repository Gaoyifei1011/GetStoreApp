using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Activation
{
    public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private INavigationService NavigationService { get; }

        public DefaultActivationHandler()
        {
            NavigationService = App.GetService<INavigationService>();
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            return NavigationService.Frame?.Content == null;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(HomeViewModel).FullName, args.Arguments);
            await Task.CompletedTask;
        }
    }
}
