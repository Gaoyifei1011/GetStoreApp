using Microsoft.UI.Xaml;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ButtonResourceDict { get; private set; }

        public static ResourceDictionary DropDownButtonResourceDict { get; private set; }

        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary FlyoutResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary InfoBarResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary PivotResourceDict { get; private set; }

        public static ResourceDictionary ScrollBarResourceDict { get; private set; }

        public static ResourceDictionary SplitButtonResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Application.Current.Resources.MergedDictionaries[1];
            DropDownButtonResourceDict = Application.Current.Resources.MergedDictionaries[2];
            ExpanderResourceDict = Application.Current.Resources.MergedDictionaries[3];
            FlyoutResourceDict = Application.Current.Resources.MergedDictionaries[4];
            GridResourceDict = Application.Current.Resources.MergedDictionaries[5];
            HyperlinkButtonResourceDict = Application.Current.Resources.MergedDictionaries[6];
            InAppNotificationResourceDict = Application.Current.Resources.MergedDictionaries[7];
            InfoBarResourceDict = Application.Current.Resources.MergedDictionaries[8];
            ListViewResourceDict = Application.Current.Resources.MergedDictionaries[9];
            MenuFlyoutResourceDict = Application.Current.Resources.MergedDictionaries[10];
            PivotResourceDict = Application.Current.Resources.MergedDictionaries[11];
            ScrollBarResourceDict = Application.Current.Resources.MergedDictionaries[12];
            SplitButtonResourceDict = Application.Current.Resources.MergedDictionaries[13];
            TextBlockResourceDict = Application.Current.Resources.MergedDictionaries[14];
        }
    }
}
