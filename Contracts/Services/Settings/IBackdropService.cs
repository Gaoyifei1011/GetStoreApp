using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IBackdropService
    {
        string AppBackdrop { get; set; }

        List<BackdropModel> BackdropList { get; set; }

        Task InitializeBackdropAsync();

        Task SetBackdropAsync(string backdrop);

        Task SetAppBackdropAsync(string appTheme, string appBackdrop);
    }
}
