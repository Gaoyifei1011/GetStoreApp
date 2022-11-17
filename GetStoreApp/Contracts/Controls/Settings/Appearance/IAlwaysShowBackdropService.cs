using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Appearance
{
    public interface IAlwaysShowBackdropService
    {
        bool AlwaysShowBackdropValue { get; set; }

        Task InitializeAlwaysShowBackdropAsync();

        Task SetAlwaysShowBackdropAsync(bool alwaysShowBackdropValue);
    }
}
