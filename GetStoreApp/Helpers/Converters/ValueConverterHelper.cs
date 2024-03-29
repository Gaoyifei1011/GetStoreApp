﻿using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 值类型 / 内容转换辅助类
    /// </summary>
    public static class ValueConverterHelper
    {
        public static bool ObjectCompareReverseConvert(object value, object comparedValue)
        {
            return !Equals(value, comparedValue);
        }

        public static Uri UriConvert(object uri)
        {
            return new Uri(uri.ToString());
        }

        /// <summary>
        /// 整数值与控件显示值转换
        /// </summary>
        public static Visibility IntToVisibilityConvert(int value)
        {
            return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 整数值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility IntToVisibilityReverseConvert(int value)
        {
            return value is 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 布尔值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility BooleanToVisibilityReverseConvert(bool value)
        {
            return value is false ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 空字符串值与显示值转换
        /// </summary>
        public static Visibility StringVisibilityConvert(string value)
        {
            return !string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 空字符串值与显示值转换（判断结果相反）
        /// </summary>
        public static Visibility StringReverseVisibilityConvert(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
