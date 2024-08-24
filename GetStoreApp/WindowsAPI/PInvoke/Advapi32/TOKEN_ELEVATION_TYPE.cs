namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    public enum TOKEN_ELEVATION_TYPE
    {
        /// <summary>
        /// 令牌没有链接令牌。
        /// </summary>
        TokenElevationTypeDefault = 1,

        /// <summary>
        /// 令牌是提升的令牌。
        /// </summary>
        TokenElevationTypeFull = 2,

        /// <summary>
        /// 令牌是有限的令牌。
        /// </summary>
        TokenElevationTypeLimited = 3
    }
}
