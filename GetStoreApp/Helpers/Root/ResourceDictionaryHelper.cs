using Microsoft.UI.Xaml;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 应用资源字典辅助类
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary DropDownButtonResourceDict { get; private set; }

        public static ResourceDictionary FlyoutResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary InfoBarResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary PivotResourceDict { get; private set; }

        public static ResourceDictionary ScrollBarResourceDict { get; private set; }

        public static ResourceDictionary SegmentedResourceDict { get; private set; }

        public static ResourceDictionary SplitButtonResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            DropDownButtonResourceDict = Application.Current.Resources.MergedDictionaries[1];
            FlyoutResourceDict = Application.Current.Resources.MergedDictionaries[2];
            HyperlinkButtonResourceDict = Application.Current.Resources.MergedDictionaries[3];
            InAppNotificationResourceDict = Application.Current.Resources.MergedDictionaries[4];
            InfoBarResourceDict = Application.Current.Resources.MergedDictionaries[5];
            ListViewResourceDict = Application.Current.Resources.MergedDictionaries[6];
            MenuFlyoutResourceDict = Application.Current.Resources.MergedDictionaries[7];
            ScrollBarResourceDict = Application.Current.Resources.MergedDictionaries[8];
            SegmentedResourceDict = Application.Current.Resources.MergedDictionaries[9];
            SplitButtonResourceDict = Application.Current.Resources.MergedDictionaries[10];
        }
    }
}
