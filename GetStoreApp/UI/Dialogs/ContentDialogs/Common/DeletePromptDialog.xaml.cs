using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Common
{
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DeleteContent { get; set; }

        public DeletePromptDialog(DeleteArgs deletePrompt)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;

            switch (deletePrompt)
            {
                case DeleteArgs.History: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteArgs.Download: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteArgs.DownloadWithFile: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
            }

            InitializeComponent();
        }
    }
}
