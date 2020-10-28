using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Goldpoint_Inventory_System
{
    class ExpanderListViewModel : INotifyPropertyChanged
    {
        private Object _selectedExpander;

        public Object SelectedExpander
        {
            get { return _selectedExpander; }
            set
            {
                if (_selectedExpander == value)
                {
                    return;
                }

                _selectedExpander = value;
                OnPropertyChanged("SelectedExpander");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
