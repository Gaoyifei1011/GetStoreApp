using GetStoreApp.Extensions.DataType.Enums;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace GetStoreApp.Services.Root
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
        /// 获取存储的数据类型
        /// </summary>
        public static StorageDataKind GetStorageDataKind()
        {
            return resultContainer.Values.TryGetValue(nameof(StorageDataKind), out object storeageDataValue) && storeageDataValue is StorageDataKind storageDataKind ? storageDataKind : StorageDataKind.None;
        }

        /// <summary>
        /// 读取结果存储信息
        /// </summary>
        public static List<string> ReadResult(StorageDataKind dataKind)
        {
            List<string> resultList = [];

            if (Equals(resultContainer.Values[nameof(StorageDataKind)], dataKind.ToString()) && resultContainer.Containers.TryGetValue(parameter, out ApplicationDataContainer parameterContainer))
            {
                for (int index = 0; index < parameterContainer.Values.Count; index++)
                {
                    if (parameterContainer.Values.TryGetValue(string.Format("Data{0}", index + 1), out object value))
                    {
                        resultList.Add(Convert.ToString(value));
                    }
                }
            }

            return resultList;
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
