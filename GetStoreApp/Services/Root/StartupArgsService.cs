using CommandLine;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.CommandLine;
using GetStoreApp.Helpers.Root;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    public class StartupArgsService : IStartupArgsService
    {
        private IResourceService ResoureService = ContainerHelper.GetInstance<IResourceService>();

        private readonly string CommandParameter = "getstoreapp:///";

        private readonly string[] CommandLineArgs = Environment.GetCommandLineArgs().Where((source, index) => index != 0).ToArray();

        // 应用启动时使用的参数
        public Hashtable StartupArgs
        { get; set; } = new Hashtable
        {
            {"TypeName",-1 },
            {"ChannelName",-1 },
            {"Link",null},
        };

        /// <summary>
        /// 处理应用启动时的参数
        /// </summary>
        public async Task InitializeStartupArgsAsync()
        {
            if (CommandLineArgs.Length == 0)
            {
                return;
            }
            else if (CommandLineArgs.Length == 1)
            {
                if (!CommandLineArgs[0].Equals(CommandParameter, StringComparison.OrdinalIgnoreCase))
                {
                    StartupArgs["Link"] = CommandLineArgs[0];
                }
            }
            else
            {
                Parser.Default.ParseArguments<CommandOptions>(CommandLineArgs).WithParsed((options) =>
                {
                    StartupArgs["TypeName"] = ResoureService.TypeList.FindIndex(item => item.ShortName.Equals(options.TypeName, StringComparison.OrdinalIgnoreCase));
                    StartupArgs["ChannelName"] = ResoureService.ChannelList.FindIndex(item => item.ShortName.Equals(options.ChannelName, StringComparison.OrdinalIgnoreCase));
                    StartupArgs["Link"] = options.Link;
                });
            }
            await Task.CompletedTask;
        }
    }
}
