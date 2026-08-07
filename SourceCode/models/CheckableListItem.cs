using DomainAdSnooper.dto;
using System.ComponentModel;

namespace AdSnooperGui.models
{

    public class CheckableListItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool isChecked
        {
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
        public int propertyTypeValue { get; set; }

        public CheckableListItem(string displayName, bool isChecked)
        {
            this.isChecked = isChecked;
            this.displayName = displayName;
        }

        public CheckableListItem(int propertyTypeValue, string displayName, bool isChecked)
        {
            this.propertyTypeValue = propertyTypeValue;
            this.isChecked = isChecked;
            this.displayName = displayName;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
