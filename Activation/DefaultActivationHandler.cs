using GetStoreApp.Contracts.Services;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Activation
{
    public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly INavigationService _navigationService;

        public DefaultActivationHandler(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            _navigationService.NavigateTo(typeof(HomeViewModel).FullName, args.Arguments);
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return _navigationService.Frame.Content == null;
        }
    }
}
