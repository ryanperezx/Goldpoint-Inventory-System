using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Goldpoint_Inventory_System.Stock
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class ModifyInvent : UserControl
    {
        public ModifyInvent()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();

            Office2019ColorfulThemeSettings themeSettings = new Office2019ColorfulThemeSettings();
            themeSettings.PrimaryBackground = new SolidColorBrush(Colors.DarkGoldenrod);
            themeSettings.PrimaryForeground = new SolidColorBrush(Colors.White);
            themeSettings.BodyFontSize = 16;
            themeSettings.HeaderFontSize = 14;
            themeSettings.SubHeaderFontSize = 14;
            themeSettings.TitleFontSize = 14;
            themeSettings.SubTitleFontSize = 14;
            themeSettings.FontFamily = new FontFamily("Calibri");
            SfSkinManager.RegisterThemeSettings("Office2019Colorful", themeSettings);
            SfSkinManager.SetTheme(this, new Theme("Office2019Colorful"));
        }
    }
}
