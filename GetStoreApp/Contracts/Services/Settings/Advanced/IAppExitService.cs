using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Advanced
{
    public interface IAppExitService
    {
        AppExitModel AppExit { get; set; }

        List<AppExitModel> AppExitList { get; set; }

        Task InitializeAppExitAsync();

        Task SetAppExitAsync(AppExitModel appExit);
    }
}
