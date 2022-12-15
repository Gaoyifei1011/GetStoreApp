// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.Views.Controls
{
    public sealed partial class ListViewRadioFlyoutItem : RadioMenuFlyoutItem
    {
        public string CheckedItem
        {
            get { return (string)GetValue(CheckedItemProperty); }
            set
            {
                if (string.IsNullOrEmpty(Text))
                {
                    IsChecked = false;
                }
                else
                {
                    IsChecked = value == Text;
                }
                SetValue(CheckedItemProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CheckedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedItemProperty =
            DependencyProperty.Register("CheckedItem", typeof(string), typeof(ListViewRadioFlyoutItem), new PropertyMetadata(string.Empty));

        public ListViewRadioFlyoutItem()
        {
            InitializeComponent();
        }
    }
}
