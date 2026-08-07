using AdSnooperGui.common;
using AdSnooperGui.viewmodels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        public ExportWindow(ExportWindowVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            /*
            Die View kennt sein ViewModel, aber nicht umgekehrt. Ein Viewmodel kann die View nicht schließen, nur die View kann das.
            Deshalb registrieren wir unseren eventHandler beim Viewmodel, um über den Wunsch einer Fensterschließung informiert zu werden.
            Wir lassen dann durch die View schließen.
            */
            ((ExportWindowVM)DataContext).onWindowClosingEvent += WindowClosingEventHandler;

            //ruft innerhalb Viewmodel auf, wenn Close gedrückt wurde
            //Closing += ((InfoWindowVM)DataContext).voteAgainstWindowClosing;
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            ((ExportWindowVM)DataContext).saveSettings();
            ((ExportWindowVM)DataContext).onButtonExport();
        }
        //private void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ExportWindowVM)DataContext).onButtonsaveClick();
        //}

        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ((ExportWindowVM)DataContext).onButtonSelectAllProperties();
        }

        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            ((ExportWindowVM)DataContext).onButtonSelectNoneProperties();
        }

        private void ButtonInvertSelection_Click(object sender, RoutedEventArgs e)
        {
            ((ExportWindowVM)DataContext).onButtonInvertSelectionProperties();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ((ExportWindowVM)DataContext).onButtonClose();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ((ExportWindowVM)DataContext).onCloseWindow();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ((ExportWindowVM)DataContext).voteAgainstWindowClosing(this, e);
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


    }
}
