using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IBackdropService
    {
        string AppBackdrop { get; set; }

        List<BackdropModel> BackdropList { get; set; }

        Task InitializeBackdropAsync();

        Task SetBackdropAsync(string theme);

        Task SetAppBackdropAsync();
    }
}
