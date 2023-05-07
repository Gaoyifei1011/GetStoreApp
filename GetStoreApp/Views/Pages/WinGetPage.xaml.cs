using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        private unsafe void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
        {
            Guid guid = new Guid("C53A4F16-787E-42A4-B304-29EFFB4BF597");
            Guid iid = new Guid("00000000-0000-0000-C000-000000000046");
            Ole32Library.CoCreateInstance(&guid, IntPtr.Zero, CLSCTX.CLSCTX_ALL, &iid, out IntPtr obj);
            PackageManager packageManager = PackageManager.FromAbi(obj);
            List<PackageCatalogReference> catalogs = packageManager.GetPackageCatalogs().ToList();
            foreach (PackageCatalogReference item in catalogs)
            {
                Debug.WriteLine("===========================");
                Debug.WriteLine("Name:" + item.Info.Name);
                Debug.WriteLine("Argument:" + item.Info.Argument);
                Debug.WriteLine("LastUpdateTime" + item.Info.LastUpdateTime);
                Debug.WriteLine("Type:" + item.Info.Type);
            }
            Debug.WriteLine(catalogs.Count);
        }
    }
}
