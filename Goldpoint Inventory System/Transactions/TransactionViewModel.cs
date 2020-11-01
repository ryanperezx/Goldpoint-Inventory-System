using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goldpoint_Inventory_System
{
    class TransactionViewModel
    {

        private ObservableCollection<TransactionModel> _transact;
        public ObservableCollection<TransactionModel> Transact
        {
            get { return _transact; }
            set { _transact = value; }
        }

        public TransactionViewModel()
        {
            _transact = new ObservableCollection<TransactionModel>();
            this.GenerateTransaction();
        }



        private void GenerateTransaction()
        {
            _transact.Add(new TransactionModel(20200001, "Ballpen", "Ballpen", "HBW", "N/A", 5, 50, "None"));
            _transact.Add(new TransactionModel(20200002, "REAM", "Paper", "Hard Copy", "Short", 1, 500, "None"));

        }
    }
}
