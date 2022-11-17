using GetStoreApp.Models.Controls.Settings.Advanced;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Advanced
{
    public interface IAppExitService
    {
        AppExitModel AppExit { get; set; }

        List<AppExitModel> AppExitList { get; set; }

        Task InitializeAppExitAsync();

        Task SetAppExitAsync(AppExitModel appExit);
    }
}
