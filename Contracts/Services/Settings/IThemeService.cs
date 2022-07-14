using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IThemeService
    {
        string AppTheme { get; set; }

        List<ThemeModel> ThemeList { get; set; }

        Task InitializeThemeAsync();

        Task SetThemeAsync(string theme);

        Task SetAppThemeAsync();
    }
}
