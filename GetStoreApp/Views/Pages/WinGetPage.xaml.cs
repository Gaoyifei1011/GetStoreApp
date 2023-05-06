using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet ³ÌÐò°üÒ³Ãæ
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public WinGetPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
        {
            Debug.WriteLine("=================================");
            Guid guid = new Guid("C53A4F16-787E-42A4-B304-29EFFB4BF597");
            Guid iid = new Guid("00000000-0000-0000-C000-000000000046");
            Debug.WriteLine("0000001");
            Ole32Library.CoCreateInstance(ref guid, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid, out object obj);
            Debug.WriteLine("0000002");
            // PackageManager packageManager = (PackageManager)obj;
            // Debug.WriteLine("0000003");
            // PackageCatalogReference packageCatalog = packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
            // Debug.WriteLine("0000004");
            // Debug.WriteLine(packageCatalog.Info.Name);
            // Debug.WriteLine("0000005");
        }
    }
}
