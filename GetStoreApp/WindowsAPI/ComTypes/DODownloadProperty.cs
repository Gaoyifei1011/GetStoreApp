namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DODownloadProperty 枚举指定传递优化下载操作的属性 ID。 此枚举由 IDODownload 接口使用，并由包含值类型的 VARIANT 值执行。
    /// </summary>
    public enum DODownloadProperty
    {
        /// <summary>
        /// 只读。 使用此属性可获取唯一标识下载的 ID。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_Id = 0,

        /// <summary>
        /// 使用此属性可设置或获取要下载的资源的远程 URI 路径。 仅当未提供 DODownloadProperty_ContentId 时才需要此属性。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_Uri = 1,

        /// <summary>
        /// 使用此属性可设置或获取下载的唯一内容 ID。 仅当未提供 DODownloadProperty_Uri 时才需要此属性。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_ContentId = 2,

        /// <summary>
        /// 可选。 使用此属性可设置或获取下载显示名称。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_DisplayName = 3,

        /// <summary>
        /// 使用此属性可设置或获取本地路径名称以保存下载文件。 如果该路径不存在，传递优化将尝试使用调用方的权限创建它。 仅当未提供 DODownloadProperty_StreamInterface 时才需要此属性。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_LocalPath = 4,

        /// <summary>
        /// 可选。 使用此属性可设置或获取自定义 HTTP 请求标头。 传递优化将在 HTTP 文件请求操作期间包括这些标头。 标头必须已格式化为标准 HTTP 标头。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_HttpCustomHeaders = 5,

        /// <summary>
        /// 可选。 使用此属性可设置或获取 DODownloadCostPolicy 枚举值之一。 VARIANT 类型为VT_UI4。
        /// </summary>
        DODownloadProperty_CostPolicy = 6,

        /// <summary>
        /// 可选，只写。 使用此属性可设置或获取标准 WinHTTP 安全标志 (WINHTTP_OPTION_SECURITY_FLAGS) 。 VARIANT 类型为VT_UI4。
        /// 支持以下标志：
        /// * SECURITY_FLAG_IGNORE_CERT_CN_INVALID。 允许证书中的公用名无效。
        /// * SECURITY_FLAG_IGNORE_CERT_DATE_INVALID。 允许无效的证书日期。
        /// * SECURITY_FLAG_IGNORE_UNKNOWN_CA。 允许无效的证书颁发机构。
        /// * SECURITY_FLAG_IGNORE_CERT_WRONG_USAGE。 允许使用非服务器证书建立服务器的标识。
        /// * WINHTTP_ENABLE_SSL_REVOCATION。 允许 SSL 吊销。 如果设置了此标志，将忽略上述标志。
        /// </summary>
        DODownloadProperty_SecurityFlags = 7,

        /// <summary>
        /// 可选。 使用此属性可以根据下载百分比设置或获取回调频率。 VARIANT 类型为VT_UI4。
        /// </summary>
        DODownloadProperty_CallbackFreqPercent = 8,

        /// <summary>
        /// 可选。 使用此属性可以根据下载时间设置或获取回调频率。 默认值为每秒一次。 VARIANT 类型为VT_UI4。
        /// </summary>
        DODownloadProperty_CallbackFreqSeconds = 9,

        /// <summary>
        /// 可选。 使用此属性可设置或获取无进度的下载超时长度。 最小接受值为无下载活动的 60 秒。 VARIANT 类型为VT_UI4。
        /// </summary>
        DODownloadProperty_NoProgressTimeoutSeconds = 10,

        /// <summary>
        /// 可选。 使用此属性可设置或获取当前下载优先级。 VARIANT_TRUE值会将下载内容置于优先级更高的前台。 默认值为后台优先级。 VARIANT 类型为VT_BOOL。
        /// </summary>
        DODownloadProperty_ForegroundPriority = 11,

        /// <summary>
        /// 可选。 使用此属性可设置或获取当前下载阻止模式。 VARIANT_TRUE值将导致 IDODownload：：Start 被阻止，直到下载完成或发生错误。 默认值为非阻止模式。 VARIANT 类型为VT_BOOL。
        /// </summary>
        DODownloadProperty_BlockingMode = 12,

        /// <summary>
        /// 可选。 使用此属性可设置或获取指向用于下载回调的 IDODownloadStatusCallback 接口的指针。 VARIANT 类型为VT_UNKNOWN。
        /// </summary>
        DODownloadProperty_CallbackInterface = 13,

        /// <summary>
        /// 可选。 使用此属性可设置或获取指向用于流下载类型的 IStream 接口的指针。 VARIANT 类型为VT_UNKNOWN。
        /// </summary>
        DODownloadProperty_StreamInterface = 14,

        /// <summary>
        /// 可选，只写。 使用此属性可设置在 HTTP 请求操作期间使用的证书上下文。 该值必须包含CERT_CONTEXT的序列化字节。 VARIANT 类型 (VT_ARRAY |VT_UI1) 。
        /// </summary>
        DODownloadProperty_SecurityContext = 15,

        /// <summary>
        /// 可选，只写。 使用此属性可设置在 HTTP 操作期间要使用的网络令牌。 VARIANT_TRUE值将导致传递优化捕获调用方的身份令牌，VARIANT_FALSE将清除现有令牌。 默认值为已登录用户的令牌。 VARIANT 类型为VT_BOOL。
        /// </summary>
        DODownloadProperty_NetworkToken = 16,

        /// <summary>
        /// 可选。 为遥测目的设置特定的相关向量。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_CorrelationVector = 17,

        /// <summary>
        /// 可选，只写。 以 JSON 字符串的形式设置解密信息。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_DecryptionInfo = 18,

        /// <summary>
        /// 可选，只写。 设置片段哈希文件 (PHF) 位置，传递优化使用该位置对下载的内容执行运行时完整性检查。 VARIANT 类型为VT_BSTR。
        /// </summary>
        DODownloadProperty_IntegrityCheckInfo = 19,

        /// <summary>
        /// 可选。 设置一个布尔标志，指示是否必须使用段哈希文件 (PHF) 。 如果VARIANT_TRUE，如果完整性检查失败，下载将中止。 VARIANT 类型为VT_BOOL。
        /// </summary>
        DODownloadProperty_IntegrityCheckMandatory = 20,

        /// <summary>
        /// 可选。 指定总下载大小（以字节为单位）。 VARIANT 类型为VT_UI8。
        /// </summary>
        DODownloadProperty_TotalSizeBytes = 21,

        /// <summary>
        /// 使用手机网络连接时，请勿下载。
        /// </summary>
        DODownloadProperty_DisallowOnCellular = 22,

        /// <summary>
        /// 在质询时使用自定义 HTTPS 标头。
        /// </summary>
        DODownloadProperty_HttpCustomAuthHeaders = 23,

        /// <summary>
        /// Https 到 http 重定向。 默认值为 FALSE。
        /// </summary>
        DODownloadProperty_HttpAllowSecureToNonSecureRedirect = 24,

        /// <summary>
        /// 将下载信息保存到 Windows 注册表。 FALSE对于传递优化下载作业，默认值为 ;TRUE对于 BITS 样式的作业，默认值为 。
        /// </summary>
        DODownloadProperty_NonVolatile = 25
    }
}
