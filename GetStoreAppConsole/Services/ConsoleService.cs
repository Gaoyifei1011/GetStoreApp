using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Helpers;
using Windows.ApplicationModel;

namespace GetStoreAppConsole.Services
{
    public class ConsoleService : IConsoleService
    {
        // 换行符
        private string LineBreaks = "\r\n";

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        /// <summary>
        /// 控制台程序运行时初始化标题提示信息
        /// </summary>
        public void InitializeDescription()
        {
            Console.Title = ResourceService.GetLocalized("Title");

            Console.WriteLine(string.Format(ResourceService.GetLocalized("HeaderDescription1"), Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision) + LineBreaks);
            Console.WriteLine(ResourceService.GetLocalized("HeaderDescription2"));
            Console.WriteLine(ResourceService.GetLocalized("HeaderDescription3") + LineBreaks);

            Console.WriteLine(ResourceService.GetLocalized("UnfinishedNotification1"));
            Console.WriteLine(ResourceService.GetLocalized("UnfinishedNotification2") + LineBreaks);
        }
    }
}
