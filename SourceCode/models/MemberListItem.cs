using System.ComponentModel;

namespace AdSnooperGui.models
{


    /// <summary>
    /// Speichert Infos (ShortName, DistinguishedName) zu Member oder MemberOf
    /// </summary>
    public class MemberListItem
    {


        public MemberListItem(string order, string shortName, string distinguishedName, string subgroupMarker)
        {
            //this.propertyType = EAdGroupProperty.Undefined;
            this.isChecked = false;
            this.shortName = shortName;
            this.distinguishedName = distinguishedName;
            this.order = order;
            this.subgroupMarker = subgroupMarker;
        }

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

        public string shortName { get; set; }
        public string distinguishedName { get; set; }
        public string order { get; set; }
        public string subgroupMarker { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string shortName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(shortName));
            }
        }
    }

}
