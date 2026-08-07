using System.Windows;

namespace AdSnooperGui.models
{
    public class CustomMessageBoxParameter
    {
        public static readonly int MessageTypQuestion = 1;
        public static readonly int MessageTypExclamationBlue = 2;
        public static readonly int MessageTypExclamationRed = 3;

        public string title { get; set; }
        public string messageText { get; set; }
        public string inputText { get; set; }
        public int messageType { get; set; } // question, exclamationBlue, exclamationRed
        public CustomMessageBoxButtonDescriptor [] buttonList { get; set; }
        public double windowWidth { get; set; } = 0;
        public double windowHeight { get; set; } = 0;
        public Visibility isProgressBarVisibility { get; set; } = Visibility.Hidden;
        public int progressBarValue { get; set; } = 0;

    }

    public class CustomMessageBoxButtonDescriptor
    {
        public CustomMessageBoxButtonDescriptor(string label, Action<object> buttonActionCommand, bool isWindowCloser)
        {
            this.label = label;
            this.buttonActionCommand = buttonActionCommand;
            this.isWindowCloser = isWindowCloser;
        }
        public CustomMessageBoxButtonDescriptor(string label, bool isWindowCloser)
        {
            this.label = label;
            this.buttonActionCommand = null;
            this.isWindowCloser = isWindowCloser;
        }
        public CustomMessageBoxButtonDescriptor(string label, Action<object> buttonActionCommand)
        {
            this.label = label;
            this.buttonActionCommand = buttonActionCommand;
            this.isWindowCloser = true;
        }

        public string label { get; set; }
        public bool isWindowCloser { get; set; }
        public bool isDefaultKeystroke { get; set; } = false;
        public bool isCancelKeystroke { get; set; } = false;
        public Action<object> buttonActionCommand { get; set; }
    }
}
