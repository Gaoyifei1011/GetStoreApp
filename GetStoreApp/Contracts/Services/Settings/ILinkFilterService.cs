using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ILinkFilterService
    {
        bool StartWithEFilterValue { get; set; }

        bool BlockMapFilterValue { get; set; }

        Task InitializeLinkFilterValueAsnyc();

        Task SetStartsWithEFilterValueAsync(bool startWithEFilterValue);

        Task SetBlockMapFilterValueAsync(bool blockMapFilterValue);
    }
}
