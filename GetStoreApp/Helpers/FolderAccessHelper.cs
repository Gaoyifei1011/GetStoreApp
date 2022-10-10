using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Windows.Storage;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 检查选定的目录是否有写入权限
    /// </summary>
    public static class FolderAccessHelper
    {
        public static bool CanWriteToFolder(StorageFolder folder, FileSystemRights accessRights)
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
    }
}
