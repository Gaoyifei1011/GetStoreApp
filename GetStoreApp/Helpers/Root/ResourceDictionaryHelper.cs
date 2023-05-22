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

        public static ResourceDictionary CommandBarResourceDict { get; private set; }

        public static ResourceDictionary ContentDialogResourceDict { get; private set; }

        public static ResourceDictionary DropDownButtonResourceDict { get; private set; }

        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary FlyoutResourceDict { get; private set; }

        public static ResourceDictionary FontIconResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary GridViewResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InfoBarResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary MinMaxCloseControlDict { get; private set; }

        public static ResourceDictionary NavigationViewResourceDict { get; private set; }

        public static ResourceDictionary PivotResourceDict { get; private set; }

        public static ResourceDictionary ScrollViewerResourceDict { get; private set; }

        public static ResourceDictionary SplitButtonResourceDict { get; private set; }

        public static ResourceDictionary TeachingTipResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        public static ResourceDictionary ToggleSwitchResourceDict { get; private set; }

        public static ResourceDictionary WindowChromeDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];
            CheckboxResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[3];
            CommandBarResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[4];
            ContentDialogResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[5];
            DropDownButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[6];
            ExpanderResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[7];
            FlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[8];
            FontIconResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[9];
            GridResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[10];
            GridViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[11];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[12];
            InfoBarResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[13];
            ListViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[14];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[15];
            MinMaxCloseControlDict = Program.ApplicationRoot.Resources.MergedDictionaries[16];
            NavigationViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[17];
            PivotResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[18];
            ScrollViewerResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[19];
            SplitButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[20];
            TeachingTipResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[21];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[22];
            ToggleSwitchResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[23];
            WindowChromeDict = Program.ApplicationRoot.Resources.MergedDictionaries[24];
        }
    }
}
