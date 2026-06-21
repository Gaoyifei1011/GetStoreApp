using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;

namespace GetStoreApp.Views.DataTemplates
{
    /// <summary>
    /// 导航项数据模板选择器
    /// </summary>
    public partial class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NavigationViewItemTemplate { get; set; }

        public DataTemplate NavigationViewSettingsItemTemplate { get; set; }

        public DataTemplate NavigationViewItemSeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is NavigationViewItemModel navigationViewItem)
            {
                if (navigationViewItem.NavigationViewItemKind is NavigationViewItemKind.Item)
                {
                    return navigationViewItem.NavigationTag is "Settings" ? NavigationViewSettingsItemTemplate : NavigationViewItemTemplate;
                }
                else if (navigationViewItem.NavigationViewItemKind is NavigationViewItemKind.Seperator)
                {
                    return NavigationViewItemSeparatorTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
