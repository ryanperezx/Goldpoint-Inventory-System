using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System
{
    class ItemDataModel
    {
        public string date
        {
            get;
            set;
        }

        public string itemCode
        {
            get;
            set;
        }

        public string description
        {
            get;
            set;
        }

        public string type
        {
            get;
            set;
        }

        public string brand
        {
            get;
            set;
        }

        public int qty
        {
            get;
            set;
        }

        public string remarks
        {
            get;
            set;
        }

        public string size
        {
            get;
            set;
        }

        public double totalPerItem
        {
            get;
            set;
        }

        public double msrp
        {
            get;
            set;
        }

        public double price
        {
            get;
            set;
        }

        public double dealersPrice
        {
            get;
            set;
        }

        public string fastMoving
        {
            get;
            set;
        }
        
        public double criticalLvl
        {
            get;
            set;
        }

        public bool criticalState
        {
            get;
            set;
        }

        public string replacement
        {
            get;
            set;
        }

    }
}
