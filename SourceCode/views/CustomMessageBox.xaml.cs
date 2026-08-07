using AdSnooperGui.common;
using AdSnooperGui.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdSnooperGui.views
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        private CustomMessageBoxVM _viewModel = null;
        

        public CustomMessageBox(CustomMessageBoxVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            

        }



        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            /*
            Die View kennt sein ViewModel, aber nicht umgekehrt. Ein Viewmodel kann die View nicht schließen, nur die View kann das.
            Deshalb registrieren wir unseren eventHandler beim Viewmodel, um über den Wunsch einer Fensterschließung informiert zu werden.
            Wir lassen dann durch die View schließen.
            */
            _viewModel.onWindowClosingEvent += WindowClosingEventHandler;

            //ruft innerhalb Viewmodel auf, wenn Close gedrückt wurde
            Closing += _viewModel.voteAgainstWindowClosing;
            

            //messageImage.Source = ((CustomMessageBoxVM)DataContext).messageImageSource;
        }

        /// <summary>
        /// Wird ausgeführt, wenn das Viewmodel die View schließen will.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WindowClosingEventHandler(object sender, WindowClosingEventArgs e)
        {
            Close();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.executeButton2Action();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.executeButton1Action();
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.executeButton0Action();
        }

        private void window_handleKeystrokeEvents(object sender, KeyEventArgs e)
        {
            _viewModel.handleKeystrokeEvents(sender, e);
        }

        
    }
}
