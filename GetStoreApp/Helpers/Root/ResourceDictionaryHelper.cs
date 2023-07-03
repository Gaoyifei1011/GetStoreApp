using Microsoft.UI.Xaml;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ButtonResourceDict { get; private set; }

        public static ResourceDictionary CheckboxResourceDict { get; private set; }

        public static ResourceDictionary ContentDialogResourceDict { get; private set; }

        public static ResourceDictionary DropDownButtonResourceDict { get; private set; }

        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary FlyoutResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary GridViewResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary InfoBarResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary NavigationViewResourceDict { get; private set; }

        public static ResourceDictionary PivotResourceDict { get; private set; }

        public static ResourceDictionary ScrollViewerResourceDict { get; private set; }

        public static ResourceDictionary SplitButtonResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        public static ResourceDictionary WindowChromeDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Application.Current.Resources.MergedDictionaries[1];
            CheckboxResourceDict = Application.Current.Resources.MergedDictionaries[2];
            ContentDialogResourceDict = Application.Current.Resources.MergedDictionaries[3];
            DropDownButtonResourceDict = Application.Current.Resources.MergedDictionaries[4];
            ExpanderResourceDict = Application.Current.Resources.MergedDictionaries[5];
            FlyoutResourceDict = Application.Current.Resources.MergedDictionaries[6];
            GridResourceDict = Application.Current.Resources.MergedDictionaries[7];
            GridViewResourceDict = Application.Current.Resources.MergedDictionaries[8];
            HyperlinkButtonResourceDict = Application.Current.Resources.MergedDictionaries[9];
            InAppNotificationResourceDict = Application.Current.Resources.MergedDictionaries[10];
            InfoBarResourceDict = Application.Current.Resources.MergedDictionaries[11];
            ListViewResourceDict = Application.Current.Resources.MergedDictionaries[12];
            MenuFlyoutResourceDict = Application.Current.Resources.MergedDictionaries[13];
            NavigationViewResourceDict = Application.Current.Resources.MergedDictionaries[14];
            PivotResourceDict = Application.Current.Resources.MergedDictionaries[15];
            ScrollViewerResourceDict = Application.Current.Resources.MergedDictionaries[16];
            SplitButtonResourceDict = Application.Current.Resources.MergedDictionaries[17];
            TextBlockResourceDict = Application.Current.Resources.MergedDictionaries[18];
            WindowChromeDict = Application.Current.Resources.MergedDictionaries[19];
        }
    }
}
