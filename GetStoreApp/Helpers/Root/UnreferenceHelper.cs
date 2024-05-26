namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 丢弃未使用的参数辅助类，抑制 IDE0060 和 CA1822 警告
    /// </summary>
    public static class UnreferenceHelper
    {
        /// <summary>
        /// 丢弃未使用的参数
        /// </summary>
        public static void Unreference(object parameter)
        {
            _ = parameter;
        }
    }
}
