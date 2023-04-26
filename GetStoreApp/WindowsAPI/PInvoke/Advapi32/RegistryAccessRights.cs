namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 注册表访问权限枚举值
    /// </summary>
    public enum RegistryAccessRights : uint
    {
        /// <summary>查询注册表项的值所必需的。</summary>
        KEY_QUERY_VALUE = 0x0001,

        /// <summary>创建、删除或设置注册表值所必需的。</summary>
        KEY_SET_VALUE = 0x0002,

        /// <summary>创建注册表项的子项是必需的。</summary>
        KEY_CREATE_SUB_KEY = 0x0004,

        /// <summary>枚举注册表项的子项所必需的。</summary>
        KEY_ENUMERATE_SUB_KEYS = 0x0008,

        /// <summary>请求注册表项或注册表项子项的更改通知所必需的。</summary>
        KEY_NOTIFY = 0x0010,

        /// <summary>预留给系统使用。</summary>
        KEY_CREATE_LINK = 0x0020,

        /// <summary>
        /// 指示 64 位Windows上的应用程序应在 64 位注册表视图中运行。 此标志被 32 位Windows忽略。
        /// <para>请参阅 访问备用注册表视图。必须将此标志与此表中查询或访问注册表值的其他标志结合使用。</para>
        /// <para>Windows 2000：不支持此标志。</para>
        /// </summary>
        KEY_WOW64_64KEY = 0x0100,

        /// <summary>
        /// 指示 64 位Windows上的应用程序应在 32 位注册表视图中运行。 此标志被 32 位Windows忽略。
        /// <para>有关详细信息，请参阅 访问备用注册表视图。必须将此标志与此表中查询或访问注册表值的其他标志结合使用。</para>
        /// <para>Windows 2000：不支持此标志。</para>
        /// </summary>
        KEY_WOW64_32KEY = 0x0200,

        /// <summary>
        /// 合并 <see cref="STANDARD_RIGHTS_WRITE" />, <see cref="KEY_SET_VALUE" />, 和 <see cref="KEY_CREATE_SUB_KEY" /> 访问权限。
        /// </summary>
        KEY_WRITE =
            (int)StandardRight.STANDARD_RIGHTS_WRITE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY,

        /// <summary>
        /// 合并 <see cref="STANDARD_RIGHTS_READ" />, <see cref="KEY_QUERY_VALUE" />, <see cref="KEY_ENUMERATE_SUB_KEYS" />, 和 <see cref="KEY_NOTIFY" /> 值。
        /// </summary>
        KEY_READ =
            (int)StandardRight.STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY,

        /// <summary>等效于 <see cref="KEY_READ" />.</summary>
        KEY_EXECUTE = KEY_READ,

        /// <summary>
        /// 合并 <see cref="StandardRight.STANDARD_RIGHTS_REQUIRED" />, <see cref="KEY_QUERY_VALUE" />, <see cref="KEY_SET_VALUE" />, <see cref="KEY_CREATE_SUB_KEY" />, <see cref="KEY_ENUMERATE_SUB_KEYS" />, <see cref="KEY_NOTIFY" />, 和 <see cref="KEY_CREATE_LINK" /> 访问权限。
        /// </summary>
        KEY_ALL_ACCESS =
            (int)StandardRight.STANDARD_RIGHTS_REQUIRED | KEY_QUERY_VALUE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY
            | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY | KEY_CREATE_LINK,
    }
}
