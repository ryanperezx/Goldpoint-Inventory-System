﻿using System;
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
        }
    }
}