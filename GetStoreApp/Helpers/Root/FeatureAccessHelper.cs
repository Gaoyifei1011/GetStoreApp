using System;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 受限API访问辅助类
    /// </summary>
    public static class FeatureAccessHelper
    {
        private static readonly string packageFamilyName = Package.Current.Id.FamilyName;

        // Windows 11 22621 受限的 API 和对应的访问密钥
        private static readonly Dictionary<string, string> limitedAccessFeaturesDict = new()
        {
            { "com.microsoft.services.cortana.cortanaactionableinsights_v1", "nEVyyzytE6ankNk1CIAu6sZsh8vKLw3Q7glTOHB11po=" },
            { "com.microsoft.windows.applicationmodel.conversationalagent_v1", "hhrovbOc/z8TgeoWheL4RF5vLLJrKNAQpdyvhlTee6I" },
            { "com.microsoft.windows.applicationmodel.phonelinetransportdevice_v1", "cb9WIvVfhp+8lFhaSrB6V6zUBGqctteKi/f/9AIeoZ4" },
            { "com.microsoft.windows.applicationwindow", "e5a85131-319b-4a56-9577-1c1d9c781218" },
            { "com.microsoft.windows.callcontrolpublicapi_v1", "6e7e52aa-cddb-4e57-9f1c-7dd511ad7d01" },
            { "com.microsoft.windows.focussessionmanager.1", "ba3faac1-0878-4bb9-9b35-2224aa0ee7cf" },
            { "com.microsoft.windows.geolocationprovider", "6D1544E3-55CB-40D2-A022-31F24E139708" },
            { "com.microsoft.windows.holographic.keyboardcursor_v1", "FE676B8B-E396-4A80-9573-B67542840E5C" },
            { "com.microsoft.windows.holographic.shell", "527f4968-f193-419a-b91f-46b9106e1129" },
            { "com.microsoft.windows.holographic.xrruntime.1", "036EFF74-8BF2-4249-82AF-92235C6E1A10" },
            { "com.microsoft.windows.holographic.xrruntime.2", "58AA36EF-7C1A-4A56-9308-FC882F56465A" },
            { "com.microsoft.windows.richeditmath", "RDZCQjY2M0YtQkFDMi00NkIwLUI3NzEtODg4NjMxMEVENkFF" },
            { "com.microsoft.windows.shell.sharewindowcommandsource_v1", "yDvrila5HS/y8SctohQM3WJZOby8NbSoL2hEPTyIRco=" },
            { "com.microsoft.windows.system.remotedesktop.provider_v1", "2F712169EF57A9FB0D590593743819F5F47E2DD13E4D9A5458DDA77608CC5E10" },
            { "com.microsoft.windows.taskbar.pin", "4096B239A7295B635C090E647E867B5707DA6AB6CB78340B01FE4E0C8F4953D4" },
            { "com.microsoft.windows.taskbar.requestPinSecondaryTile", "04c19204-10d9-450a-95c4-2910c8f72be3" },
            { "com.microsoft.windows.ui.notifications.preview.toastOcclusionManagerPreview", "738a6acf-45c1-44ed-85a4-5eb11dc2d084" },
            { "com.microsoft.windows.updateorchestrator.1", "20C662033A4007A55375BF00D986C280B41A418F" },
            { "com.microsoft.windows.windowdecorations", "425261a8-7f73-4319-8a53-fc13f87e1717" },
        };

        /// <summary>
        /// 根据 featureId 生成 token
        /// </summary>
        public static string GenerateTokenFromFeatureId(string featureId)
        {
            string generatedContent = string.Format("{0}!{1}!{2}", featureId, limitedAccessFeaturesDict[featureId], packageFamilyName);
            byte[] computedHash = HashAlgorithmHelper.ComputeSHA256Hash(generatedContent);
            byte[] tokenBytes = new byte[16];
            Array.Copy(computedHash, tokenBytes, tokenBytes.Length);
            return Convert.ToBase64String(tokenBytes);
        }

        /// <summary>
        /// 生成声明发布者有权使用该功能的纯英语语句
        /// </summary>
        public static string GenerateAttestation(string featureId)
        {
            string[] packageFamilyNameArray = packageFamilyName.Split('_');
            return packageFamilyNameArray.Length > 0 ? string.Format("{0} has registered their use of {1} with Microsoft and agrees to the terms of use.", packageFamilyNameArray[^1], featureId) : string.Empty;
        }
    }
}
