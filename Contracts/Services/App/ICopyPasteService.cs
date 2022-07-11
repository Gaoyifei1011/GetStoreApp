using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface ICopyPasteService
    {
        void CopyStringToClipBoard(string content);
    }
}
