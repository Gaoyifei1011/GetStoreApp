using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GetStoreAppHelper.Helpers
{
    public class ResourceDictionaryHelper
    {
        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static async Task InitializeResourceDictionaryAsync()
        {
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];

            await Task.CompletedTask;
        }
    }
}
