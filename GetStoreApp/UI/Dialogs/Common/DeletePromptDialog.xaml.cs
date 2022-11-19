using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Common
{
    public sealed partial class DeletePromptDialog : ContentDialog
    {
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
