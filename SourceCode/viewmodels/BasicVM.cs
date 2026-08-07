using AdSnooperGui.common;
using AdSnooperGui.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdSnooperGui.viewmodels
{
    public class BasicVM : INotifyPropertyChanged
    {
        //error validation
        //-> INotifyDataErrorInfo
        //public Dictionary<string, string> _validationErrors = new Dictionary<string, string>();
        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        //public bool HasErrors => _validationErrors.Count > 0;
        //public IEnumerable GetErrors(string propertyName) =>
        //    _validationErrors.TryGetValue(propertyName, out string error) ? new string[1] { error } : null;
        //protected void sendValidationErrorEvent(DataErrorsChangedEventArgs eventArgs)
        //{
        //    ErrorsChanged?.Invoke(this, eventArgs);
        //}
        //--

        public string windowTitle { get; set; }
        public double windowLeft { get; set; }
        public double windowTop { get; set; }
        public double windowWidth { get; set; }
        public double windowHeight { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<TableDataChangedEventArgs> OnTableDataChangedEvent;
        protected void sendTableDataChangedEvent(TableDataChangedEventArgs eventArgs)
        {
            OnTableDataChangedEvent?.Invoke(this, eventArgs);
        }


        /// <summary>
        /// Wenn das Viewmodel die View schließen möchte, wird das per Event an die View gemeldet.
        /// Nur die entscheidet und schließt sich selbst, weil wir die View nicht kennen.
        /// </summary>
        public event EventHandler<WindowClosingEventArgs> onWindowClosingEvent;
        void sendWindowClosingEvent(WindowClosingEventArgs eventArgs)
        {
            onWindowClosingEvent?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Wenn wir die View schließen möchten
        /// </summary>
        public void closeWindow()
        {
            sendWindowClosingEvent(new WindowClosingEventArgs());
        }
        public void closeWindow(WindowClosingEventArgs windowClosingEventArgs)
        {
            sendWindowClosingEvent(windowClosingEventArgs);
        }


    }
}
