using System;

namespace GetStoreApp.Contracts.Services.Shell
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}