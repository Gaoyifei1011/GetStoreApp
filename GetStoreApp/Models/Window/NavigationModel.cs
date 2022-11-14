using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.Models.Window
{
    public class NavigationModel : ModelBase
    {
        /// <summary>
        /// 页面标签
        /// </summary>
        public string NavigationTag
        {
            get { return (string)GetValue(NavigationTagProperty); }
            set { SetValue(NavigationTagProperty, value); }
        }

        public static readonly DependencyProperty NavigationTagProperty =
            DependencyProperty.Register("NavigationTag", typeof(string), typeof(NavigationModel), new PropertyMetadata(string.Empty));

        public NavigationViewItem NavigationItem
        {
            get { return (NavigationViewItem)GetValue(NavigationItemProperty); }
            set { SetValue(NavigationItemProperty, value); }
        }

        public static readonly DependencyProperty NavigationItemProperty =
            DependencyProperty.Register("NavigationItem", typeof(int), typeof(NavigationModel), new PropertyMetadata(default));

        /// <summary>
        /// 页面类型
        /// </summary>
        public Type NavigationPage
        {
            get { return (Type)GetValue(NavigationPageProperty); }
            set { SetValue(NavigationPageProperty, value); }
        }

        public static readonly DependencyProperty NavigationPageProperty =
            DependencyProperty.Register("NavigationPage", typeof(Type), typeof(NavigationModel), new PropertyMetadata(default));
    }
}
