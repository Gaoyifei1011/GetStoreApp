using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.DataTemplates
{
    /// <summary>
    /// 操作栏数据模版选择器
    /// </summary>
    internal sealed partial class WinGetAppInfoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringInfoTemplate { get; set; }

        public DataTemplate UriInfoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is WinGetAppInfoModel winGetAppInfoItem)
            {
                if (winGetAppInfoItem.WinGetAppInfoKind is WinGetAppInfoKind.String)
                {
                    return StringInfoTemplate;
                }
                else if (winGetAppInfoItem.WinGetAppInfoKind is WinGetAppInfoKind.Uri)
                {
                    return UriInfoTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
