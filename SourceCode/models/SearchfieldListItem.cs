using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdSnooperGui.models
{
    /// <summary>
    /// Item einer Liste von Suchfeldern
    /// </summary>
    public class SearchfieldListItem : INotifyPropertyChanged
    {
        //Todo ESearchfieldLabelId entfenen
        //public ESearchfieldLabelId labelId { get; set; } // veraltet, kommt bald weg
        public int adAttributeId { get; set; } //neu: ist die Attribute-ID aus den AppSettings der AD-Attribute-Liste
        public string labelName { get; set; }
        public string tabIndex { get; set; }
        public List<string> autoCompleteSuggestionList { get; set; }

        private string _textFieldValue = null;
        public string textFieldValue 
        { 
            get{ return _textFieldValue; }
            set
            {
                _textFieldValue = value;
                OnPropertyChanged("textFieldValue");
            }
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
