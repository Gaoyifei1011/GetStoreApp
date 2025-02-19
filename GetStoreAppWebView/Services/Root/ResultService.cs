using GetStoreAppWebView.Extensions.DataType.Enums;
using System.Collections.Generic;
using Windows.Storage;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 存储运行结果服务
    /// </summary>
    public static class ResultService
    {
        private const string result = "Result";
        private const string parameter = "Parameter";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer resultContainer;

        /// <summary>
        /// 初始化结果记录存储服务
        /// </summary>
        public static void Initialize()
        {
            resultContainer = localSettingsContainer.CreateContainer(result, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 保存结果存储信息
        /// </summary>
        public static void SaveResult(StorageDataKind dataKind, List<string> dataList)
        {
            resultContainer.Values[nameof(StorageDataKind)] = dataKind.ToString();

            if (dataKind is StorageDataKind.None)
            {
                if (resultContainer.Containers.ContainsKey(parameter))
                {
                    resultContainer.DeleteContainer(parameter);
                }
            }
            else
            {
                if (dataList is not null)
                {
                    ApplicationDataContainer parameterContainer = resultContainer.CreateContainer(parameter, ApplicationDataCreateDisposition.Always);

                    for (int index = 0; index < dataList.Count; index++)
                    {
                        parameterContainer.Values[string.Format("Data{0}", index + 1)] = dataList[index];
                    }
                }
            }
        }
    }
}
