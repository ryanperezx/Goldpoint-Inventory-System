using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for Photocopy.xaml
    /// </summary>
    public partial class Photocopy : UserControl
    {
        ObservableCollection<PhotocopyDataModel> items = new ObservableCollection<PhotocopyDataModel>();

        public Photocopy()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgPhotocopy.ItemsSource = items;
            items.Add(new PhotocopyDataModel
            {
                item = "Long",
                price = .80,
                qty = 5,
                totalPerItem = 5
            });
            items.Add(new PhotocopyDataModel
            {
                item = "Short",
                price = .70,
                qty = 5,
                totalPerItem = 3.5
            });
            items.Add(new PhotocopyDataModel
            {
                item = "A4",
                price = .90,
                qty = 5,
                totalPerItem = 4.5
            });


        }


    }
}
