using AdSnooperGui.models;
using AdSnooperGui.viewmodels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsWindowVM _viewModel;
        public SettingsWindow(SettingsWindowVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageListItem item = cbLanguage.SelectedItem as LanguageListItem;
            ((SettingsWindowVM)DataContext).onLanguageChanged(item);
        }

        private void Button_Click_WriteOrderfile(object sender, RoutedEventArgs e)
        {
            _viewModel.onActionWriteOrderfile();
        }
        private void Button_Click_ImportOrderfile(object sender, RoutedEventArgs e)
        {
            _viewModel.onActionImportOrderfile();
        }

        private void cbLicenseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.onLicenssTypeChanged();
        }
    }
}
