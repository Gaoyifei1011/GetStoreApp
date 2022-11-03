﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 关于界面项目引用和感谢介绍文字提示转换器
    /// </summary>
    public class AboutToolTipFormatConverter : IValueConverter
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string param = System.Convert.ToString(parameter);

            if (param == "Reference")
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("/About/ReferenceToolTip"));
            }
            else if (param == "Thanks")
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("/About/ThanksToolTip"));
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
