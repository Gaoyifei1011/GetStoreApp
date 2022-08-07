using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class ResultControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public ResultViewModel ViewModel { get; }

        public ResultControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<ResultViewModel>();

            InitializeComponent();
        }

        public string LocalizedCategoryId(string categoryId)
        {
            return string.Format(ResourceService.GetLocalized("/Home/categoryId"), categoryId);
        }

        public string LocalizedResultCountInfo(int count)
        {
            return string.Format(ResourceService.GetLocalized("/Home/ResultCountInfo"), count);
        }
    }
}
