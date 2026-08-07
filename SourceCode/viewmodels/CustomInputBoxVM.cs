using AdSnooperGui.appsettings;
using AdSnooperGui.models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AdSnooperGui.viewmodels
{
    public class CustomInputBoxVM : BasicVM
    {
        public string title { get; set; }
        public string messageText { get; set; }

        private string _inputText = null;
        public string inputText 
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(inputText));
            }
        }

        // buttons
        public string button0Label { get; set; }
        public string button1Label { get; set; }
        public Visibility button0Visibility { get; set; }
        public Visibility button1Visibility { get; set; }
        public string imageSource { get; set; }
        public Action<object> button0ActionCommand { get; set; }
        public Action<object> button1ActionCommand { get; set; }


        private CustomMessageBoxParameter _parameter = null;
        private AppSettings appSettings = null;
        private bool isButtonActionExecuted = false;//verhindert die doppelte Ausführung der Action beim Schließen per X
        private CustomMessageBoxButtonDescriptor[] buttonList;

        public CustomInputBoxVM()
        {
        }
        public CustomInputBoxVM(CustomMessageBoxParameter parameter, AppSettings appSettings)
        {
            _parameter = parameter;
            this.appSettings = appSettings;
            init();
        }

        private void init()
        {
            title = _parameter.title + " - AD Snooper";
            messageText = _parameter.messageText;
            inputText = _parameter.inputText;
            buttonList = _parameter.buttonList;
            //image
            if (_parameter.messageType == CustomMessageBoxParameter.MessageTypQuestion)
                imageSource = "/images/QuestionmarkBlue.png";
            else if (_parameter.messageType == CustomMessageBoxParameter.MessageTypExclamationBlue)
                imageSource = "/images/ExclamationmarkBlue.png";
            else if (_parameter.messageType == CustomMessageBoxParameter.MessageTypExclamationRed)
                imageSource = "/images/ExclamationmarkRed.png";
            else
                imageSource = "/images/QuestionmarkBlue.png";

            //buttons
            if (_parameter.buttonList.Length == 1)
            {
                button0Label = _parameter.buttonList[0].label;
                button0Visibility = Visibility.Visible;
                button1Visibility = Visibility.Hidden;
            }
            else if (_parameter.buttonList.Length == 2)
            {
                button0Label = _parameter.buttonList[0].label;
                button1Label = _parameter.buttonList[1].label;
                button0Visibility = Visibility.Visible;
                button1Visibility = Visibility.Visible;
            }
        }

        public void executeButton0Action()
        {
            if (_parameter.buttonList[0] != null)
            {
                isButtonActionExecuted = true;
                if (_parameter.buttonList[0].buttonActionCommand != null)
                    _parameter.buttonList[0].buttonActionCommand("btn0");
                if (_parameter.buttonList[0].isWindowCloser)
                    closeWindow();
            }
        }
        public void executeButton1Action()
        {
            if (_parameter.buttonList[1] != null)
            {
                isButtonActionExecuted = true;
                _parameter.inputText = inputText;
                if (_parameter.buttonList[1].buttonActionCommand != null)
                    _parameter.buttonList[1].buttonActionCommand("btn1");
                if (_parameter.buttonList[1].isWindowCloser)
                    closeWindow();
            }
        }

        public void handleKeystrokeEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_parameter.buttonList[0].isDefaultKeystroke)
                    executeButton0Action();
                if (_parameter.buttonList.Length >= 2 && _parameter.buttonList[1].isDefaultKeystroke)
                    executeButton1Action();
            }
            if (e.Key == Key.Escape)
            {
                if (_parameter.buttonList[0].isCancelKeystroke)
                    executeButton0Action();
                if (_parameter.buttonList.Length >= 2 && _parameter.buttonList[1].isCancelKeystroke)
                    executeButton1Action();
            }
        }

        /// <summary>
        /// Hier kann das Schließen verhindert werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void voteAgainstWindowClosing(object sender, CancelEventArgs e)
        {
            //if(isUnsavedData)
            //    e.Cancel = true;
            //e.Cancel = false;
            if (!isButtonActionExecuted && _parameter.buttonList[0] != null && _parameter.buttonList[0].buttonActionCommand != null)
                _parameter.buttonList[0].buttonActionCommand("btn0");
            e.Cancel = false;

        }

    }
}
