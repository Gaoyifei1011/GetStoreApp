using GetStoreAppConsole.Strings;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Windows.Globalization;

namespace GetStoreAppConsole
{
    public static class ConsoleHelper
    {
        // 换行符
        public static string LineBreaks = "\r\n";

        /// <summary>
        /// 控制台程序运行时初始化标题提示信息
        /// </summary>
        public static void InitializeDescription()
        {
            Console.Title = StringLocalization.Title;

            Console.WriteLine(string.Format(StringLocalization.HeaderDescription1, Assembly.GetExecutingAssembly().GetName().Version) + LineBreaks);
            Console.WriteLine(StringLocalization.HeaderDescription2);
            Console.WriteLine(StringLocalization.HeaderDescription3 + LineBreaks);

            Console.WriteLine(StringLocalization.UnfinishedNotification1);
            Console.WriteLine(StringLocalization.UnfinishedNotification2 + LineBreaks);
        }

        /// <summary>
        /// 初始化控制台程序的本地化资源内容
        /// </summary>
        public static void InitializeLocalization()
        {
            // 获取应用设置设定的语言
            string AppLanguage = ApplicationLanguages.PrimaryLanguageOverride;

            // 设置控制台程序使用的语言
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(AppLanguage);
        }
    }
}
