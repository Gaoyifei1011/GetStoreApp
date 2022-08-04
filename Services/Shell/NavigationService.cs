using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 导航服务
    /// </summary>
    public class NavigationService : INavigationService
    {
        private object _lastParameterUsed;
        private Frame _frame;

        private IPageService PageService { get; } = GetStoreApp.App.GetService<IPageService>();

        public event NavigatedEventHandler Navigated;

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = GetStoreApp.App.MainWindow.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= OnNavigated;
            }
        }

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                _frame.GoBack();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }

            return false;
        }

        public bool NavigateTo(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null, bool clearNavigation = false)
        {
            var pageType = PageService.GetPageType(pageKey);

            if (_frame.Content?.GetType() != pageType || parameter != null && !parameter.Equals(_lastParameterUsed))
            {
                _frame.Tag = clearNavigation;
                var vmBeforeNavigation = _frame.GetPageViewModel();
                var navigated = _frame.Navigate(pageType, parameter, infoOverride);
                if (navigated)
                {
                    _lastParameterUsed = parameter;
                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        public void CleanNavigation()
            => _frame.BackStack.Clear();

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.BackStack.Clear();
                }

                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }

                Navigated?.Invoke(sender, e);
            }
        }
    }
}
