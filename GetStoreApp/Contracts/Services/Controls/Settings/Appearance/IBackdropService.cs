using GetStoreApp.Models.Controls.Settings.Appearance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Controls.Settings.Appearance
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
