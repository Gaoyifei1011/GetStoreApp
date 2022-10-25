using CommandLine;

namespace GetStoreApp.Extensions.CommandLine
{
    /// <summary>
    /// 应用启动的参数
    /// </summary>
    public class CommandOptions
    {
        [Option('t', "type", Required = false)]
        public string TypeName { get; set; }

        [Option('c', "channel", Required = false)]
        public string ChannelName { get; set; }

        [Option('l', "link", Required = true)]
        public string Link { get; set; }
    }
}
