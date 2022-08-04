using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class ResultControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public ResultViewModel ViewModel { get; set; }

        public string Test { get; set; } = "testtsring";

        public ResultControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<ResultViewModel>();
            this.DataContext = ViewModel;
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
