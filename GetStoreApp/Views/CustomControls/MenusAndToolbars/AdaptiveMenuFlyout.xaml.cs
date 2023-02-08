using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.CustomControls.MenusAndToolbars
{
    /// <summary>
    /// 自适应系统主题变化的菜单浮动控件
    /// </summary>
    public partial class AdaptiveMenuFlyout : MenuFlyout
    {
        public AdaptiveMenuFlyout()
        {
            InitializeComponent();
            Opened += OnOpened;
        }

        ~AdaptiveMenuFlyout()
        {
            Opened -= OnOpened;
        }

        public void OnOpened(object sender, object args)
        {
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                {
                    MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutLightPresenter"] as Style;
                }
                else
                {
                    MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutDarkPresenter"] as Style;
                }
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutLightPresenter"] as Style;
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutDarkPresenter"] as Style;
            }
        }
    }
}
