namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 安装应用类型
    /// </summary>
    internal enum InstallAppsKind
    {
        None = 0,
        NonPackagedApp = 1,
        PackagedAppViaAppInstaller = 2,
        PackagedAppDirectlyInstall = 3,
    }
}
