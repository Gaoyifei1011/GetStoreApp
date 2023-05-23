using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.CustomControls.DialogsAndFlyouts
{
    /// <summary>
    /// 自适应系统主题变化的浮出控件
    /// </summary>
    public sealed partial class AdaptiveFlyout : Flyout
    {
        public AdaptiveFlyout()
        {
            InitializeComponent();
            Opened += OnOpened;
        }

        ~AdaptiveFlyout()
        {
            Opened -= OnOpened;
        }

        public void OnOpened(object sender, object args)
        {
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    FlyoutPresenterStyle = ResourceDictionaryHelper.FlyoutResourceDict["AdaptiveFlyoutLightPresenterStyle"] as Style;
                }
                else
                {
                    FlyoutPresenterStyle = ResourceDictionaryHelper.FlyoutResourceDict["AdaptiveFlyoutDarkPresenterStyle"] as Style;
                }
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                FlyoutPresenterStyle = ResourceDictionaryHelper.FlyoutResourceDict["AdaptiveFlyoutLightPresenterStyle"] as Style;
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                FlyoutPresenterStyle = ResourceDictionaryHelper.FlyoutResourceDict["AdaptiveFlyoutDarkPresenterStyle"] as Style;
            }
        }
    }
}
