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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg2MDY1QDMxMzgyZTM0MmUzMEJZUmNoeFdncVJkd2ZuK09STmo1S2VURFhXMWJtOTNiQVFhQzFpQkU3dlU9");
            SfSkinManager.ApplyStylesOnApplication = true;

        }
    }
}
