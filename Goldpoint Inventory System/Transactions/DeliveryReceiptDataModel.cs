using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System.Transactions
{
    class DeliveryReceiptDataModel
    {

        public int qty
        {
            get;
            set;
        }

        public string description
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
