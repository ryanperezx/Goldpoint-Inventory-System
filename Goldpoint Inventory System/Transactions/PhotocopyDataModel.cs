using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System.Transactions
{
    class PhotocopyDataModel
    {
        public string item
        {
            get;
            set;
        }

        public int qty
        {
            get;
            set;
        }

        public double price
        {
            get;
            set;
        }

        public double totalPerItem
        {
            get;
            set;
        }
    }
}
