using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Contracts.Services.Shell
{
    public interface INavigationService
    {
        event NavigatedEventHandler Navigated;

        bool CanGoBack { get; }

        Frame Frame { get; set; }

        bool NavigateTo(string pageKey, object parameter, NavigationTransitionInfo infoOverride, bool clearNavigation);

        bool GoBack();
    }
}
