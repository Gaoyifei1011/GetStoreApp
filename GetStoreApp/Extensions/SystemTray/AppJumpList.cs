using Windows.UI.StartScreen;

namespace GetStoreApp.Extensions.SystemTray
{
    /// <summary>
    /// 应用任务栏跳转列表
    /// </summary>
    public static class AppJumpList
    {
        // 应用跳转列表组名称
        public static string GroupName { get; set; }

        // 应用跳转列表类型
        public static JumpListSystemGroupKind GroupKind { get; set; }
    }
}
