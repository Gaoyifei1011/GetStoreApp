using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IConfigService
    {
        Task<string> GetSettingStringValueAsync(string key);

        Task<bool?> GetSettingBoolValueAsync(string key);

        Task SaveSettingStringValueAsync(string key, string value);

        Task SaveSettingBoolValueAsync(string key, bool value);
    }
}
