using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Home
{
    /// <summary>
    /// 复制文字提醒转化器
    /// Copy the text alert converter
    /// </summary>
    public class CopyToolTipConverter : IValueConverter
    {
        /// <summary>
        /// 为复制的文字添加上可复制提醒
        /// Adds a copyable reminder on the copied text
        /// </summary>
        /// <returns>为文字添加上可复制提醒的内容</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string result = value as string;

            return result + HomeViewModel.CopyToolTip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}