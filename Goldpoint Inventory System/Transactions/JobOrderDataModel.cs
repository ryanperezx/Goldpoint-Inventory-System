using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System.Transactions
{
    class JobOrderDataModel
    {
        public string unit
        {
            get;
            set;
        }

        public string description
        {
            get;
            set;
        }

        public int qty
        {
            get;
            set;
        }

        public string copy
        {
            get;
            set;
        }

        public string size
        {
            get;
            set;
        }

        public string material
        {
            get;
            set;
        }

        public double unitPrice
        {
            get;
            set;
        }

        public double amount
        {
            get;
            set;
        }


    }
}
