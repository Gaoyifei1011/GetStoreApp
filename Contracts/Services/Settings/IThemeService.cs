using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
