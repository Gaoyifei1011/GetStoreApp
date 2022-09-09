using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IBackdropService
    {
        BackdropModel AppBackdrop { get; set; }

        List<BackdropModel> BackdropList { get; set; }

        Task InitializeBackdropAsync();

        Task SetBackdropAsync(BackdropModel backdrop);

        Task SetAppBackdropAsync();
    }
}
