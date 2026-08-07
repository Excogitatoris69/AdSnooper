using AdSnooperGui.models;
using AdSnooperGui.viewmodels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdSnooperGui.views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM _viewModel = null;
        public MainWindow(MainWindowVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            ((MainWindowVM)DataContext).OnTableDataChangedEvent += OnTableDataChangedEvent;
            Closing += _viewModel.OnWindowClosing;//Registriert diese VM-Methode, um über das X-Closing-Event informiert zu werden
        }

        protected override void OnContentRendered(EventArgs e)
        {
            //memberOfGroupFilter
            CollectionView dataGridMemberOfGroupCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGridMemberOfGroup.ItemsSource);
            dataGridMemberOfGroupCollectionView.Filter = memberOfGroupFilter;
            //memberOfGroupFilter2
            CollectionView dataGridMemberOfGroup2CollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGridGroupMemberlist.ItemsSource);
            dataGridMemberOfGroup2CollectionView.Filter = memberOfGroup2Filter;
            //groupResultFilter
            CollectionView dataGridGroupResultCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGridGroupResult.ItemsSource);
            dataGridGroupResultCollectionView.Filter = groupFilter;
            initSearchPersonResultTable();
            initSearchGroupResultTable();
            initGroupMemberTable();
            initMemberOfTable();
            base.OnContentRendered(e);
            
        }

        private void initSearchPersonResultTable()
        {
            foreach (DynamicDataTableColumn tabColumn in _viewModel.personSearchResultTableColumnList)
            {
                dataGridPersonResult.Columns.Add(
                    new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding), Width = new DataGridLength(tabColumn.width) }
                );
            }
        }

        private void initSearchGroupResultTable()
        {
            foreach (DynamicDataTableColumn tabColumn in _viewModel.groupSearchResultTableColumnsList)
            {
                dataGridGroupResult.Columns.Add(
                    new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding), Width = new DataGridLength(tabColumn.width) }
                );
            }
        }

        private void initGroupMemberTable()
        {
            /*
            //Liest die Spaltenname aus dem VM
            foreach (DynamicDataTableColumn tabColumn in _viewModel.groupMemberTableColumnList)
            {
                dataGridGroupMemberlist.Columns.Add(
                    new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding), Width = new DataGridLength(tabColumn.width) }
                );
            }
            */
        }
        //dataGridMemberOfGroup
        private void initMemberOfTable()
        {
            /*
            //Liest die Spaltenname aus dem VM
            foreach (DynamicDataTableColumn tabColumn in _viewModel.personMemberOfTableColumnList)
            {
                dataGridMemberOfGroup.Columns.Add(
                    new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding), Width = new DataGridLength(tabColumn.width) }
                );
            }
            */
        }


        private bool memberOfGroupFilter(object item)
        {
            if (String.IsNullOrEmpty(tbGroupMemberTextFilter.Text))
                return true;//
            else
            {
                string displayName = (item as MemberListItem).shortName;
                bool displaNameMatched = displayName != null && displayName.IndexOf(tbGroupMemberTextFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                return displaNameMatched;
            }
        }

        private bool memberOfGroup2Filter(object item)
        {
            if (String.IsNullOrEmpty(tbGroupMemberTextFilter2.Text))
                return true;
            else
            {
                /*
                string personalId= (item as DataTablePersonModel).personalId;
                string firstName = ((DataTablePersonModel)item).firstName;
                string lastName = (item as DataTablePersonModel).lastName;
                string department = (item as DataTablePersonModel).department;
                bool personalidMatched = personalId!=null && personalId.IndexOf(tbGroupMemberTextFilter2.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                bool firstNameMatched = firstName!=null && firstName.IndexOf(tbGroupMemberTextFilter2.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                bool lastNameMatched = lastName!=null && lastName.IndexOf(tbGroupMemberTextFilter2.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                bool departmentMatched = department!=null && department.IndexOf(tbGroupMemberTextFilter2.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                return personalidMatched || firstNameMatched || lastNameMatched || departmentMatched;
                */
                string displayName = (item as MemberListItem).shortName;
                bool displaNameMatched = displayName != null && displayName.IndexOf(tbGroupMemberTextFilter2.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                return displaNameMatched;

            }
        }

        private bool groupNameSuggestionFilter(object item)
        {
            //tbGroupnamesearchField
            return true;
        }

        /// <summary>
        /// Filter der Ergebnistabelle bei Gruppensuche
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool groupFilter(object item)
        {
            if (String.IsNullOrEmpty(tbGroupTextFilter.Text))
                return true;
            else
            {
                // Item ist vom Typ DynamicDataTableRow
                //Man muss in allen Spalten suchen
                string displayName = (item as DynamicDataTableRow).getFullRowContent();
                bool displaNameMatched = displayName != null && displayName.IndexOf(tbGroupTextFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                return displaNameMatched;
                //return ((item as DataTableGroupModel).name.IndexOf(tbGroupTextFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void tbGroupMemberTextFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGridMemberOfGroup.ItemsSource).Refresh();
        }

        private void tbGroupMemberTextFilter2_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGridGroupMemberlist.ItemsSource).Refresh();
        }

        private void tbGroupTextFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGridGroupResult.ItemsSource).Refresh();
        }

        /// <summary>
        /// Wird beim X-Close-Klick zuletzt aufgerufen.
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);
        //    //((MainWindowVM)DataContext).onCloseWindow(); //ruft VM_ethode auf beim Closed
        //}

        /// <summary>
        /// Wird beim X-Close-Klick zuerst aufgerufen.
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //}


        /// <summary>
        /// Wenn es Änderungen gab an einer TAbelle, wird dessen erste Zeile markiert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTableDataChangedEvent(object? sender, TableDataChangedEventArgs e)
        {
            if(e.tableId== TableDataChangedEventArgs.TABLE_ID_PERSONSEARCHRESULT)
                dataGridPersonResult.SelectedIndex = e.rowIndex;
            else if (e.tableId == TableDataChangedEventArgs.TABLE_ID_GROUPSEARCHRESULT)
                dataGridGroupResult.SelectedIndex = e.rowIndex;
        }


        /// <summary>
        /// Speichert alle selektierten Zeilen der Tabelle dataGridPersonResult
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridPersonResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridPersonResult.SelectedItems;
            _viewModel.currentSelectedPersonTableList.Clear();
            foreach (DynamicDataTableRow item in allItems)
            {
                _viewModel.currentSelectedPersonTableList.Add(item);
            }
            _viewModel.onSelectionChangedTablePerson();
        }

        private void DataGridPersonProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridPersonProperties.SelectedItems;
            _viewModel.currentSelectedPersonsPropertiesList.Clear();
            foreach (PersonPropertyListItem item in allItems)
            {
                _viewModel.currentSelectedPersonsPropertiesList.Add(item);
            }
            _viewModel.onSelectionChangedTablePersonProperties();

        }

        private void DataGridPersonGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridMemberOfGroup.SelectedItems;
            _viewModel.currentSelectedPersonsGroupsList.Clear();
            foreach (MemberListItem item in allItems)
            {
                _viewModel.currentSelectedPersonsGroupsList.Add(item);
            }
            
        }


        /// <summary>
        /// Speichert alle selektierten Zeilen der Tabelle dataGridGroupResult
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridGroupResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridGroupResult.SelectedItems;
            _viewModel.currentSelectedGroupTableList.Clear();
            foreach (DynamicDataTableRow item in allItems)
            {
                _viewModel.currentSelectedGroupTableList.Add(item);
            }
            _viewModel.onSelectionChangedTableGroup();
        }

        /// <summary>
        /// Speichert alle selektierten Zeilen der Tabelle dataGridGroupResult
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridGroupMemberlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList allItems = dataGridGroupMemberlist.SelectedItems;
            _viewModel.currentSelectedGroupsMemberList.Clear();
            foreach (object item in allItems)
            {
                _viewModel.currentSelectedGroupsMemberList.Add((MemberListItem)item);
            }
        }

        /// <summary>
        /// Wenn an den Spaltenbreiten etwas verändert wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridPersonResult_LayoutUpdated(object sender, EventArgs e)
        {
            for (int colIndex = 0; colIndex < dataGridPersonResult.Columns.Count; colIndex++)
            {
                if (dataGridPersonResult.Columns[colIndex].ActualWidth != _viewModel.columnSizesPersonSearchResult[colIndex])
                {
                    _viewModel.columnSizesPersonSearchResult[colIndex] = dataGridPersonResult.Columns[colIndex].ActualWidth;
                }
            }
        }
    }
}
