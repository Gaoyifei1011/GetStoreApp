using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Appearance
{
    public interface IThemeService
    {
        ThemeModel AppTheme { get; set; }

        List<ThemeModel> ThemeList { get; set; }

        Task InitializeThemeAsync();

        Task SetThemeAsync(ThemeModel theme);

        Task SetAppThemeAsync();
    }
}
