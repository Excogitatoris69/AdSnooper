using AdSnooperGui.common;
using DomainAdSnooper.dto;
using System.ComponentModel;

namespace AdSnooperGui.models
{
    public class PersonPropertyListItem : INotifyPropertyChanged
    {
        
        private bool _isChecked;
        public bool isChecked {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("isChecked");
            }
        }
        public string displayName { get; set; } //propertyName
        public string propertyValue { get; set; }
        public string order { get; set; }
        public PersonPropertyListItem(string order, string displayName, string propertyValue)
        {
            this.isChecked = false;
            this.displayName = displayName;
            this.propertyValue = propertyValue;
            this.order = order;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string? ToString()
        {
            return this.displayName;
        }
    }

}
