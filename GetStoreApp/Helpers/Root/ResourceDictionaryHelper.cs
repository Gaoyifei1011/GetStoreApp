using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ButtonResourceDict { get; set; }

        public static ResourceDictionary CheckboxResourceDict { get; set; }

        public static ResourceDictionary CommandBarResourceDict { get; set; }

        public static ResourceDictionary ContentDialogResourceDict { get; set; }

        public static ResourceDictionary DropDownButtonResourceDict { get; set; }

        public static ResourceDictionary ExpanderResourceDict { get; set; }

        public static ResourceDictionary FontIconResourceDict { get; set; }

        public static ResourceDictionary GridResourceDict { get; set; }

        public static ResourceDictionary GridViewResourceDict { get; set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; set; }

        public static ResourceDictionary InfoBarResourceDict { get; set; }

        public static ResourceDictionary ListViewResourceDict { get; set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; set; }

        public static ResourceDictionary NavigationViewResourceDict { get; set; }

        public static ResourceDictionary PivotResourceDict { get; set; }

        public static ResourceDictionary TeachingTipResourceDict { get; set; }

        public static ResourceDictionary TextBlockResourceDict { get; set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static async Task InitializeResourceDictionaryAsync()
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
            NavigationViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[15];
            PivotResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[16];
            TeachingTipResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[17];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[18];

            await Task.CompletedTask;
        }
    }
}
