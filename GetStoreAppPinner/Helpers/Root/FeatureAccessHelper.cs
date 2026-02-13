using GetStoreApp.Helpers.Root;
using GetStoreAppPinner.WindowsAPI.PInvoke.Advapi32;
using Windows.ApplicationModel;

namespace GetStoreAppPinner.Helpers.Root
{
    /// <summary>
    /// 受限功能访问辅助类
    /// </summary>
    public static class FeatureAccessHelper
    {
        private static readonly string packageFamilyName = Package.Current.Id.FamilyName;

        /// <summary>
        /// 获取功能对应的键值
        /// </summary>
        public static string GetFeatureId(string feature)
        {
            return RegistryHelper.ReadRegistryKey<string>(ReservedKeyHandles.HKEY_LOCAL_MACHINE, string.Format(@"{0}\{1}", @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModel\LimitedAccessFeatures", feature), null);
        }

        /// <summary>
        /// 根据 featureId 生成 token
        /// </summary>
        public static string GenerateTokenFromFeatureId(string feature, string featureId)
        {
            string generatedContent = string.Format("{0}!{1}!{2}", feature, featureId, packageFamilyName);
            return HashAlgorithmHelper.ComputeSHA256Hash(generatedContent);
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
