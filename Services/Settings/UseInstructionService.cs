using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    public class UseInstructionService : IUseInstructionService
    {
        private readonly IConfigService ConfigService;

        private const string SettingsKey = "UseInsVisValue";

        private bool DefaultUseInsVisValue { get; } = true;

        public bool UseInsVisValue { get; set; }

        public UseInstructionService(IConfigService configService)
        {
            ConfigService = configService;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的使用说明按钮显示值
        /// </summary>
        public async Task InitializeUseInsVIsValueAsync()
        {
            UseInsVisValue = await GetUseInsVisValueAsync();
        }

        /// <summary>
        /// 获取设置存储的使用说明按钮显示值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetUseInsVisValueAsync()
        {
            bool? useInsVisValue = await ConfigService.GetSettingBoolValueAsync(SettingsKey);

            if (!useInsVisValue.HasValue) return DefaultUseInsVisValue;

            return Convert.ToBoolean(useInsVisValue);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public async Task SetUseInsVisValueAsync(bool useInsVisValue)
        {
            UseInsVisValue = useInsVisValue;

            await ConfigService.SaveSettingBoolValueAsync(SettingsKey, useInsVisValue);
        }
    }
}
