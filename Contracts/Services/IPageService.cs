using System;

namespace GetStoreApp.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}