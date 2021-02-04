using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Goldpoint_Inventory_System.Log
{
    public class SalesDataModel 
    {
        public string date
        {
            get;
            set;
        }

        public string desc
        {
            get;
            set;
        }

        public int qty
        {
            get;
            set;
        }

        public double amount
        {
            get;
            set;
        }

        public double total
        {
            get;
            set;
        }

        public string status
        {
            get;
            set;
        }

    }
}
