using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class ThanksViewModel : ObservableRecipient
    {
        public Dictionary<string, string> ThanksDict => new Dictionary<string, string>
        {
            {"AndromedaMelody","https://github.com/AndromedaMelody" },
            {"飞翔","https://fionlen.azurewebsites.net/"},
            {"MouriNaruto","https://github.com/MouriNaruto" },
            {"TaylorShi","https://github.com/TaylorShi" }
        };
    }
}
