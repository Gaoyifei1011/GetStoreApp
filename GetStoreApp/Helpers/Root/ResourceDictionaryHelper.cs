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
            FontIconResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[8];
            GridResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[9];
            GridViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[10];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[11];
            InfoBarResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[12];
            ListViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[13];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[14];
            MinMaxCloseControlDict = Program.ApplicationRoot.Resources.MergedDictionaries[15];
            NavigationViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[16];
            PivotResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[17];
            ScrollViewerResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[18];
            TeachingTipResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[19];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[20];
            ToggleSwitchResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[21];
            WindowChromeDict = Program.ApplicationRoot.Resources.MergedDictionaries[22];
        }
    }
}
