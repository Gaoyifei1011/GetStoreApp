namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DODownloadCostPolicy 枚举指定与 DODownloadProperty_CostPolicy 属性关联的成本策略选项的 ID。
    /// </summary>
    public enum DODownloadCostPolicy
    {
        /// <summary>
        /// 下载运行而不考虑成本。
        /// </summary>
        DODownloadCostPolicy_Always = 0,

        /// <summary>
        /// 下载运行，除非施加成本或流量限制。
        /// </summary>
        DODownloadCostPolicy_Unrestricted = 1,

        /// <summary>
        /// 下载运行，除非既不收取附加费，也不接近耗尽。
        /// </summary>
        DODownloadCostPolicy_Standard = 2,

        /// <summary>
        /// 下载运行，除非该连接需要支付漫游附加费。
        /// </summary>
        DODownloadCostPolicy_NoRoaming = 3,

        /// <summary>
        /// 下载运行，除非收取附加费。
        /// </summary>
        DODownloadCostPolicy_NoSurcharge = 4,

        /// <summary>
        /// 除非网络位于手机网络，否则下载将运行。
        /// </summary>
        DODownloadCostPolicy_NoCellular = 5
    }
}
