using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Windows.Storage;

namespace GetStoreApp.Helpers
{
    public static class IOHelper
    {
        /// <summary>
        /// 检查选定的目录是否有写入权限
        /// </summary>
        public static bool GetFolderAuthorization(StorageFolder folder, FileSystemRights accessRights)
        {
            bool CanWrite = false;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder.Path);
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                AuthorizationRuleCollection collection = directorySecurity.GetAccessRules(true, true, typeof(NTAccount));

                WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(currentUser);

                foreach (AuthorizationRule rule in collection)
                {
                    if (rule is not FileSystemAccessRule fsAccessRule)
                    {
                        continue;
                    }

                    if ((fsAccessRule.FileSystemRights & accessRights) > 0)
                    {
                        NTAccount ntAccount = rule.IdentityReference as NTAccount;
                        if (ntAccount == null)
                        {
                            continue;
                        }

                        if (principal.IsInRole(ntAccount.Value))
                        {
                            if (fsAccessRule.AccessControlType == AccessControlType.Deny)
                            {
                                CanWrite = false;
                            }
                            else
                            {
                                CanWrite = true;
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                CanWrite = false;
            }

            return CanWrite;
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="folder"></param>
        public static bool CleanFolder(StorageFolder folder)
        {
            try
            {
                if (string.IsNullOrEmpty(folder.Path) || !Directory.Exists(folder.Path))
                {
                    return true;
                }

                // 删除当前文件夹下所有文件
                foreach (string strFile in Directory.GetFiles(folder.Path))
                {
                    File.Delete(strFile);
                }
                // 删除当前文件夹下所有子文件夹(递归)
                foreach (string strDir in Directory.GetDirectories(folder.Path))
                {
                    Directory.Delete(strDir, true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取文件的SHA1值
        /// </summary>
        public static string GetFileSHA1(string filePath)
        {
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous);
                SHA1 sha1 = SHA1.Create();
                byte[] retval = sha1.ComputeHash(fileStream);
                fileStream.Close();

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < retval.Length; i++)
                {
                    stringBuilder.Append(retval[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
