using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.viewmodels;
using CoreAdSnooper.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Controls;

namespace AdSnooperGui.UserControls
{
    /// <summary>
    /// Interaction logic for SqlQuery.xaml
    /// </summary>
    public partial class UCQueryView : UserControl
    {
        private UCQueryViewVM viewmodel = null;

        public UCQueryView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            try
            {
                IDataService dataService = App.serviceProvider.GetRequiredService<IDataService>();
                ViewManager viewManager = App.serviceProvider.GetRequiredService<ViewManager>();
                

                viewmodel = new UCQueryViewVM(dataService, viewManager);
                DataContext = viewmodel;

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Wenn ein EditorTab selectiert wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tcEditorTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewmodel.editorTabControlSelectionChanged(tcEditorTabControl.SelectedIndex);
        }

        /// <summary>
        /// Wenn ein Listeneintrag in der Batchliste selektiert weird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridBatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewmodel.onBatchListSelectionChanged();
        }

        private void dataGridBatchList_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewmodel.openTabItem();
        }

        private void ButtonDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dataGridBatchList.Focus();
        }
    }
}
