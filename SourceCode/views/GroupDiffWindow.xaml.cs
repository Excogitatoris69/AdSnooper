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
    /// Interaction logic for GroupDiffWindow.xaml
    /// </summary>
    public partial class GroupDiffWindow : Window
    {
        private GroupDiffWindowVM _viewModel = null;
        public GroupDiffWindow(GroupDiffWindowVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
            init();
        }
        protected override void OnContentRendered(EventArgs e)
        {
            //GroupFilter
            CollectionView dataGridGroupDiffCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGridGroupDiff.ItemsSource);
            dataGridGroupDiffCollectionView.Filter = groupFilter;
        }

        private bool groupFilter(object item)
        {
            if (String.IsNullOrEmpty(tbGroupTextFilter.Text))
                return true;
            else
                return ((item as DynamicDataTableRow).cellData[1].IndexOf(tbGroupTextFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void tbGroupTextFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGridGroupDiff.ItemsSource).Refresh();
        }

        private void init()
        {
            foreach(DynamicDataTableColumn tabColumn in _viewModel.columnList)
            {
                dataGridGroupDiff.Columns.Add(
                    new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding) , Width = new DataGridLength(tabColumn.width)}
                );
            }
        }

        /// <summary>
        /// Speichert alle selektierten Zeilen der Tabelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridGroupDiff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridGroupDiff.SelectedItems;
            _viewModel.currentSelectedRowList.Clear();
            foreach (DynamicDataTableRow item in allItems)
            {
                _viewModel.currentSelectedRowList.Add(item);
            }
            //_viewModel.onSelectionChangedTablePerson();
        }
    }
}
