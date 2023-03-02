namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 要通过 <see cref=" User32Library.GetAncestor" /> 获取祖先。
    /// </summary>
    public enum GetAncestorFlags
    {
        /// <summary>
        /// 检索父窗口。 这不包括所有者，因为它与 GetParent 函数一样。</summary>
        GA_PARENT = 1,

        /// <summary>通过走父窗口链来检索根窗口。</summary>
        GA_ROOT = 2,

        /// <summary>
        /// 通过走 GetParent 返回的父窗口和所有者窗口链来检索拥有的根窗口。
        /// </summary>
        GA_ROOTOWNER = 3,
    }
}
