using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Syncfusion;
using Syncfusion.SfSkinManager;

namespace Goldpoint_Inventory_System
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzc2NzA2QDMxMzgyZTM0MmUzMFY2OHM0SFVKK05TWFhwc0wxWEtKU3VzbDUzc2VyVjVsQ0lSUksyTW83SlE9");
            SfSkinManager.ApplyStylesOnApplication = true;

        }
    }
}
