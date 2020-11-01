using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System
{
    class TransactionModel
    {
        int itemCode;
        string itemName;
        string type;
        string brand;
        string size;
        int qty;
        double total;
        string remarks;

        public int ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        public string Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; }
        }

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        public TransactionModel(int itemCode, string itemName, string type, string brand, string size, int qty, double total, string remarks)
        {
            this.itemCode = itemCode;
            this.itemName = itemName;
            this.type = type;
            this.brand = brand;
            this.size = size;
            this.qty = qty;
            this.total = total;
            this.remarks = remarks;
        }

    }
}
