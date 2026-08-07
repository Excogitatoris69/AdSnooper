using AdSnooperGui.appsettings;
using AdSnooperGui.models;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace AdSnooperGui.viewmodels
{


    public class CustomMessageBoxVM : BasicVM
    {
        public string title { get; set; }
        public string messageText { get; set; }
        
        // buttons
        public string button0Label { get; set; }
        public string button1Label { get; set; }
        public string button2Label { get; set; }
        public Visibility button0Visibility { get; set; }
        public Visibility button1Visibility { get; set; }
        public Visibility button2Visibility { get; set; }
        public string imageSource { get; set; }
        public Action<object> button0ActionCommand { get; set; }
        public Action<object> button1ActionCommand { get; set; }

        public bool button0isDefault { get; set; }
        public bool button1isDefault { get; set; }
        public bool button2isDefault { get; set; }

        public Visibility isProgressBarVisibility { get; set; }

        public int progressBarMaxValue { get; set; }
        private int _progressBarValue;
        public int progressBarValue 
        {
            get {  return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                progressBarMaxValue = value;
                if (_progressBarValue > 0)
                {
                    isProgressBarVisibility = Visibility.Visible;
                    progressbarTimer = new System.Timers.Timer(_progressBarValue*1000);
                    progressbarTimer.Interval = 1000;
                    button0Visibility = Visibility.Hidden;
                    progressbarTimer.Elapsed += OnTimedEvent;
                    progressbarTimer.Enabled = true;
                }
            }
        }
        

        private CustomMessageBoxParameter _parameter=null;
        private AppSettings appSettings = null;
        private bool isButtonActionExecuted = false;//verhindert die doppelte Ausführung der Action beim Schließen per X
        private CustomMessageBoxButtonDescriptor[] buttonList;
        private System.Timers.Timer progressbarTimer;

        public CustomMessageBoxVM(CustomMessageBoxParameter parameter, AppSettings appSettings)
        {
            _parameter = parameter;
            this.appSettings = appSettings;
            init();
        }

        public CustomMessageBoxVM()
        {
            
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _progressBarValue--;
            OnPropertyChanged(nameof(progressBarValue));
            if (_progressBarValue <= 0)
            {
                progressbarTimer.Enabled = false;
                progressbarTimer.Stop();
                button0Visibility = Visibility.Visible;
                OnPropertyChanged(nameof(button0Visibility));
                isProgressBarVisibility = Visibility.Hidden;
                OnPropertyChanged(nameof(isProgressBarVisibility));
            }
            
        }

        private void init()
        {
            title = _parameter.title + " - AD Snooper";
            messageText = _parameter.messageText;
            buttonList = _parameter.buttonList;
            isProgressBarVisibility = Visibility.Hidden;
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
            button0isDefault = false;
            button1isDefault = false;
            button2isDefault = false;
            if (_parameter.buttonList.Length == 1)
            {
                button0Label = _parameter.buttonList[0].label;
                button0Visibility = Visibility.Visible;
                button1Visibility = Visibility.Hidden;
                button2Visibility = Visibility.Hidden;
                if (_parameter.buttonList[0].isDefaultKeystroke)
                {
                    button0isDefault = true;
                    button1isDefault = false;
                    button2isDefault = false;
                }
            }
            else if (_parameter.buttonList.Length == 2)
            {
                button0Label = _parameter.buttonList[0].label;
                button1Label = _parameter.buttonList[1].label;
                button0Visibility = Visibility.Visible;
                button1Visibility = Visibility.Visible;
                button2Visibility = Visibility.Hidden;
                if (_parameter.buttonList[0].isDefaultKeystroke)
                {
                    button0isDefault = true;
                    button1isDefault = false;
                    button2isDefault = false;
                }
                if (_parameter.buttonList[1].isDefaultKeystroke)
                {
                    button0isDefault = false;
                    button1isDefault = true;
                    button2isDefault = false;
                }
            }
            else if (_parameter.buttonList.Length == 3)
            {
                button0Label = _parameter.buttonList[0].label;
                button1Label = _parameter.buttonList[1].label;
                button2Label = _parameter.buttonList[2].label;
                button0Visibility = Visibility.Visible;
                button1Visibility = Visibility.Visible;
                button2Visibility = Visibility.Visible;
                if (_parameter.buttonList[0].isDefaultKeystroke)
                {
                    button0isDefault = true;
                    button1isDefault = false;
                    button2isDefault = false;
                }
                if (_parameter.buttonList[1].isDefaultKeystroke)
                {
                    button0isDefault = false;
                    button1isDefault = true;
                    button2isDefault = false;
                }
                if (_parameter.buttonList[2].isDefaultKeystroke)
                {
                    button0isDefault = false;
                    button1isDefault = false;
                    button2isDefault = true;
                }
            }

            


        }


        


        public void executeButton0Action()
        {
            if(_parameter.buttonList[0] != null)
            {
                isButtonActionExecuted = true;
                if(_parameter.buttonList[0].buttonActionCommand != null)
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
                if (_parameter.buttonList[1].buttonActionCommand != null)
                    _parameter.buttonList[1].buttonActionCommand("btn1");
                if(_parameter.buttonList[1].isWindowCloser)
                    closeWindow();
            }
        }
        public void executeButton2Action()
        {
            if (_parameter.buttonList[2] != null)
            {
                isButtonActionExecuted = true;
                if (_parameter.buttonList[2].buttonActionCommand != null)
                    _parameter.buttonList[2].buttonActionCommand("btn2");
                if (_parameter.buttonList[2].isWindowCloser)
                    closeWindow();
            }
        }

        public void handleKeystrokeEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_parameter.buttonList[0].isDefaultKeystroke)
                    executeButton0Action();
                if (_parameter.buttonList.Length >=2 && _parameter.buttonList[1].isDefaultKeystroke)
                    executeButton1Action();
                if (_parameter.buttonList.Length >= 3 && _parameter.buttonList[2].isDefaultKeystroke)
                    executeButton2Action();
            }
            if (e.Key == Key.Escape)
            {
                if (_parameter.buttonList[0].isCancelKeystroke)
                    executeButton0Action();
                if (_parameter.buttonList.Length >= 2 && _parameter.buttonList[1].isCancelKeystroke)
                    executeButton1Action();
                if (_parameter.buttonList.Length >= 3 && _parameter.buttonList[2].isCancelKeystroke)
                    executeButton2Action();
            }
        }
        
        
        /// <summary>
        /// Hier kann das Schließen verhindert werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void voteAgainstWindowClosing(object sender, CancelEventArgs e)
        {
            if (_progressBarValue > 0)
            {
                e.Cancel = true;
                return;
            }
            //e.Cancel = false;
            if (!isButtonActionExecuted && _parameter.buttonList[0] != null && _parameter.buttonList[0].buttonActionCommand != null)
                _parameter.buttonList[0].buttonActionCommand("btn0");
            e.Cancel = false;

        }

    }
}
