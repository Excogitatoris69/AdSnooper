using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using CoreAdSnooper.common;
using CoreAdSnooper.interfaces;
using CoreAdSnooper.services;
using DomainAdSnooper.dto;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace AdSnooperGui.viewmodels
{

    public class MainWindowVM : BasicVM
    {
        private IDataService dataService;
        private ViewManager viewManager = null;
        private Cursor currentCursor=null;
        public AppSettings appSettings { get; set; }
        private CancellationTokenSource searchCancellationTokenSource = null;
        private int debugTaskDelayTime = 0;

        public double[] columnSizesPersonSearchResult { get; set; }

        //Personen
        public ObservableCollection<DynamicDataTableRow> personSearchResultTableList { get; set; } //Suchergebnistabelle f. Personen
        public ObservableCollection<DynamicDataTableColumn> personSearchResultTableColumnList { get; set; }//Spaltenliste der Tabelle personSearchResultTableList
        public ObservableCollection<PersonPropertyListItem> personPropertyList { get; set; } //Liste aller Properties mit Name und Wert
        public ObservableCollection<MemberListItem> personGroupMemberList { get; set; } // Alle Gruppen, zu denen eine Person gehört (Member of)
        //public ObservableCollection<DynamicDataTableColumn> personMemberOfTableColumnList { get; set; } // Spaltenliste der Tabelle MemberOf bei Usersearch (Member of)
        public ObservableCollection<SearchfieldListItem> searchPersonFieldList { get; set; }  //Suchfelder bei Personensuche
        public List<DynamicDataTableRow> currentSelectedPersonTableList=null;//alle aktuell selektierten Einträge in der Personensuchergenisliste
        public List<PersonPropertyListItem> currentSelectedPersonsPropertiesList = null;
        public List<MemberListItem> currentSelectedPersonsGroupsList = null;
        private List<AdPersonDto> resultListPerson = null;

        //Group
        //public ObservableCollection<DataTableGroupModel> groupSearchResultList { get; set; }   //Suchergebnis bei Gruppensuche
        public ObservableCollection<DynamicDataTableRow> groupSearchResultTableList { get; set; }   //Suchergebnis bei Gruppensuche
        public ObservableCollection<DynamicDataTableColumn> groupSearchResultTableColumnsList { get; set; }   //Spaltenliste der Tabelle groupSearchResultTableList
        public ObservableCollection<MemberListItem> groupMemberList { get; set; }  // Mitgliederpersonen einer Gruppe (member)
        //public ObservableCollection<DynamicDataTableColumn> groupMemberTableColumnList { get; set; }  // Spaltenliste der Tabelle GroupMember (member)
        public ObservableCollection<DynamicDataTableRow> groupMemberTableList { get; set; }  // Mitgliederpersonen einer Gruppe (member)
        public List<SearchfieldListItem> searchGroupFieldList { get; set; }  //Suchfelder bei Gruppensuche
        //public List<DataTableGroupModel> currentSelectedGroupList = null;
        public List<DynamicDataTableRow> currentSelectedGroupTableList = null;
        public List<MemberListItem> currentSelectedGroupsMemberList = null;
        private List<AdGroupDto> resultListGroup = null;



        public double A005_SearchPersonAreaWith { get; set; } = 300;
        public Visibility isProgressBarSearchGroupVisibility { get; set; }
        public int progressBarSearchGroupValue { get; set; }
        private Dictionary<string, ExportJobParameterDto> exportWindowSettingsDic = null;


        // commands
        private bool commandSearchEnabled = true;
        public GenericCommand cmdSearch { get; set; }
        public GenericCommand cmdCancelSearch { get; set; }
        public GenericCommand cmdClearSearchfields { get; set; }
        public GenericCommand cmdOpenSettingsWindow { get; set; }
        public GenericCommand cmdOpenPersonExportWindow { get; set; }
        public GenericCommand cmdOpenGroupExportWindow { get; set; }
        public GenericCommand cmdOpenHelpWindow { get; set; }   
        public GenericCommand cmdOpenGroupDiffWindow { get; set; }
        
        public GenericCommand cmdPersonCopyClipboard { get; set; }
        public GenericCommand cmdPersonMailCopyClipboard { get; set; }
        public GenericCommand cmdPersonPropertiesCopyClipboard { get; set; }
        public GenericCommand cmdPersonGroupsCopyClipboard { get; set; }
        public GenericCommand cmdGroupCopyClipboard { get; set; }
        public GenericCommand cmdGroupNameCopyClipboard { get; set; }
        public GenericCommand cmdGroupsMemberCopyClipboard { get; set; }


        public MainWindowVM()
        {
        }

        public MainWindowVM(IDataService dataService, ViewManager viewManager, AppSettings appSettings )
        {
            this.dataService = dataService;
            this.viewManager = viewManager;
            this.appSettings = appSettings;
            init();


            
        }

        private void init()
        {
            if (appSettings.B001_Language == ELanguageSet.German.ToString())
            {
                var vCulture = new CultureInfo("de-DE");
                Resources.Culture = vCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }
            else
            {
                var vCulture = new CultureInfo("en-US");
                Resources.Culture = vCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }
            windowTitle = "AD Snooper";

           


        }

        /// <summary>
        /// Erstellt die Tabellenspalten der Personensuchergebnistabelle
        /// </summary>
        private void initPersonSearchResultTable()
        {
            List<AdAttributeDto> attributeList = appSettings.getSearchResultUserAttributesList();
            int columnsCount = attributeList.Count;

            columnSizesPersonSearchResult = appSettings.getColumnSizesPersonSearchResult();
            string language = appSettings.getLanguageSetShort();
            personSearchResultTableColumnList = new ObservableCollection<DynamicDataTableColumn>();
            personSearchResultTableColumnList.Add(new DynamicDataTableColumn("#", string.Format("cellData[{0}]", 0), columnSizesPersonSearchResult[0]));//#
            for (int c = 0; c < columnsCount; c++)
            {
                personSearchResultTableColumnList.Add(new DynamicDataTableColumn(attributeList[c].getLabel(language), string.Format("cellData[{0}]", c + 1), columnSizesPersonSearchResult[c]));
            }
        }


        /// <summary>
        /// Erstellt die Tabellenspalten der Gruppensuchergebnistabelle
        /// </summary>
        private void initGroupSearchResultTable()
        {
            List<AdAttributeDto> attributeList = appSettings.getSearchResultGroupAttributesList();
            int columnsCount = attributeList.Count;
            string language = appSettings.getLanguageSetShort();
            groupSearchResultTableColumnsList = new ObservableCollection<DynamicDataTableColumn>();
            groupSearchResultTableColumnsList.Add(new DynamicDataTableColumn("#", string.Format("cellData[{0}]", 0), 50));//#
            for (int c = 0; c < columnsCount; c++)
            {
                groupSearchResultTableColumnsList.Add(new DynamicDataTableColumn(attributeList[c].getLabel(language), string.Format("cellData[{0}]", c + 1), 250));
            }
        }

        /// <summary>
        /// Erstellt die ColumnList der Tabelle Gruppen-Member
        /// Wird nicht benötigt, da statisch.
        /// </summary>
        //private void initGroupMemberTable()
        //{
        //    List<AdAttributeDto> attributeList = appSettings.getMemberGroupAttributesList();
        //    int columnsCount = attributeList.Count;
        //    string language = appSettings.getLanguageSetShort();
        //    groupMemberTableColumnList = new ObservableCollection<DynamicDataTableColumn>();
        //    groupMemberTableColumnList.Add(new DynamicDataTableColumn("#", string.Format("cellData[{0}]", 0), 50));//#
        //    for (int c = 0; c < columnsCount; c++)
        //    {
        //        groupMemberTableColumnList.Add(new DynamicDataTableColumn(attributeList[c].getLabel(language), string.Format("cellData[{0}]", c + 1), 250));
        //    }
        //}

        /// <summary>
        /// Wird immer aufgerufen, wenn der DataService ein Event sendet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataService_onDataServiceEvent(object? sender, DataServiceEventArgs e)
        {
            if(e.eventId == DataServiceEventArgs.EVENT_ID_SEARCH_GROUPMEMBER_START)
            {
                progressBarSearchGroupValue = 0;
                OnPropertyChanged("progressBarSearchGroupValue");
                //isProgressBarSearchGroupVisibility = Visibility.Visible;
                OnPropertyChanged("isProgressBarSearchGroupVisibility");
            }
            if (e.eventId == DataServiceEventArgs.EVENT_ID_SEARCH_GROUPMEMBER_FINISHED)
            {
                //isProgressBarSearchGroupVisibility = Visibility.Hidden;
                OnPropertyChanged("isProgressBarSearchGroupVisibility");
                commandSearchEnabled = true;
            }
            if (e.eventId == DataServiceEventArgs.EVENT_ID_SEARCH_GROUPMEMBER_PROGRESSBAR_VALUE)
            {
                progressBarSearchGroupValue = e.progressBarValue;
                OnPropertyChanged("progressBarSearchGroupValue");

            }
            if (e.eventId == DataServiceEventArgs.EVENT_ID_SEARCH_GROUPMEMBER_FIRSTDATA_AVAILABLE)
            {
                //Marker FIRSTDATA_AVAILABLE
                Application.Current.Dispatcher.Invoke(
                          DispatcherPriority.Normal,
                          new Action(() => {
                              generateGroupMemberList();
                              //sendTableDataChangedEvent(new TableDataChangedEventArgs { rowIndex = 0, tableId = TableDataChangedEventArgs.TABLE_ID_GROUPSEARCHRESULT });
                          }));
            }
        }

        private void OnTableDataChanged(object? sender, TableDataChangedEventArgs e)
        {
            //int x = 0;
            showBusyMouse(false);
        }


        private bool canActionOpenSettingsWindow(object arg)
        {
            return true;
        }

        private void onActionOpenSettingsWindow(object obj)
        {
            //string temp1 = Properties.Resources.L008_PersonalId;
            viewManager.openSettingswindow();
        }

        private bool canActionOpenPersonExportWindow(object arg)
        {
            return resultListPerson!=null && resultListPerson.Count>0;
        }
        private bool canActionOpenGroupExportWindow(object arg)
        {
            return resultListGroup != null && resultListGroup.Count > 0;
        }

        private void onActionOpenPersonOrGroupExportWindow(object param)
        {
            string? actionParam = param as string;
            ExportJobParameterDto exportWindowParameter = null;
            string language = appSettings.getLanguageSetShort();
            int rowIndex = 0;
            int colIndex = 0;
            if (actionParam != null && actionParam.Equals(Constants.CommandParameterPerson))
            {
               
            }
            if (actionParam != null && actionParam.Equals(Constants.CommandParameterGroup))
            {
                
            }
            
        }

        

        private bool canActionOpenGroupDiffWindow(object arg)
        {
            return currentSelectedPersonTableList.Count > 1;
        }
        private void onActionOpenGroupDiffWindow(object param)
        {
            
            //Marker onActionOpenGroupDiffWindow
            GroupDiffWindowParameter groupDiffWindowParameter = new GroupDiffWindowParameter();
            groupDiffWindowParameter.columnWidth = new double[currentSelectedPersonTableList.Count];
            
            viewManager.openGroupDiffWindow(groupDiffWindowParameter);
        }

        private void generateDataTablePersonSearchResult(List<AdPersonDto> resultList)
        {
            
        }


        private void generateDataTableGroupSearchResult(List<AdGroupDto> resultList)
        {
            int numberOfDigits = (int)Math.Floor(Math.Log10(resultList.Count) + 1);
            List<AdAttributeDto> attributeList = appSettings.getSearchResultGroupAttributesList();
            int columnsCount = attributeList.Count;
            string language = appSettings.getLanguageSetShort();
            groupSearchResultTableList.Clear();
            if (resultList != null)
            {
                
            }
        }

        /// <summary>
        /// Erstellt Übersicht der Menmber einer ausgewählten Gruppe
        /// </summary>
        private void generateGroupMemberList()
        {
            groupMemberList.Clear();
            AdGroupDto foundGroup = null;
            int index = currentSelectedGroupTableList[0].rowNumber;
            if (resultListGroup.Count > 0)
                foundGroup = resultListGroup[index];
            if (foundGroup != null)
            {
                string language = appSettings.getLanguageSetShort();
                int orderIndex = 0;
                int numberOfDigits = (int)Math.Floor(Math.Log10(foundGroup.memberList.Count) + 1);//Anzahl Ziffern in Abhängigkeit der größtmöglichen Zahl
                foreach (MemberDto aMemberOfDto in foundGroup.memberList)
                {
                    orderIndex++;
                    groupMemberList.Add(new MemberListItem(orderIndex.ToString("D" + numberOfDigits), 
                        aMemberOfDto.shortName, aMemberOfDto.distinguishedName, aMemberOfDto.fromSubgroup? Resources.L078_Yes: ""));
                }
            }

            
        }

        /// <summary>
        /// Erstellt die Properties-Liste auf Grundlage des selektierten Tabelleneintrags aus der Resulttabelle.
        /// </summary>
        private void generatePersonPropertyList()
        {
            personPropertyList.Clear();
            personGroupMemberList.Clear();
            AdPersonDto foundPerson = null;
            int index = currentSelectedPersonTableList[0].rowNumber;
            if(resultListPerson.Count>0) 
                foundPerson = resultListPerson[index];
            if (foundPerson != null)
            {
                string language = appSettings.getLanguageSetShort();
                int orderIndex = 0;
                int numberOfDigits = (int)Math.Floor(Math.Log10(appSettings.adAttributeListUser.Count) + 1);//Anzahl Ziffern in Abhängigkeit der größtmöglichen Zahl
                foreach (AdAttributeDto attributItem in appSettings.adAttributeListUser)
                {
                    orderIndex++;
                    personPropertyList.Add(new PersonPropertyListItem(orderIndex.ToString("D" + numberOfDigits), attributItem.getLabel(language), foundPerson.dataDictionary[attributItem.id]));
                }
                if (foundPerson.memberofList != null)
                {
                    orderIndex = 0;
                    numberOfDigits = (int)Math.Floor(Math.Log10(foundPerson.memberofList.Count) + 1);
                    foreach (MemberDto aMemberOfDto in foundPerson.memberofList)
                    {
                        orderIndex++;
                        personGroupMemberList.Add(new MemberListItem(orderIndex.ToString("D" + numberOfDigits), 
                            aMemberOfDto.shortName, aMemberOfDto.distinguishedName, aMemberOfDto.fromSubgroup ? Resources.L078_Yes : ""));
                    }
                }
            }
        }


        /// <summary>
        /// Liefert aus der resultListPerson alle AdPersonDto mit dem gesuchten Index.
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        private List<AdPersonDto> getPersonListById(int[] idList)
        {
            List<AdPersonDto> result = new List<AdPersonDto>();
            foreach(int idItem in idList)
            {
                result.Add(resultListPerson[idItem]);
            }
            return result;
        }

        


        /// <summary>
        /// On select row in Table TablePersonSearchResult
        /// </summary>
        /// <param name="selectedItem"></param>
        public void onSelectionChangedTablePerson()
        {
            if (currentSelectedPersonTableList.Count == 1) 
            {
                generatePersonPropertyList();
            }
            else
            {
                personPropertyList.Clear();
                personGroupMemberList.Clear();
            }
        }

        public void onSelectionChangedTablePersonProperties()
        {

        }

        public void onSelectionChangedTableGroup()
        {
            if (currentSelectedGroupTableList.Count == 1)
            {
                generateGroupMemberList();
            }
            else
            {
                groupMemberList.Clear();
            }
        }

        /// <summary>
        /// Erstellt einen Kommaseparierten String aller Name(cn)-Attribute der ausgewählten user
        /// und speichert das im Clipboard.
        /// </summary>
        /// <param name="param"></param>
        private async void onActionPersonNameCopyClipboard(object param)
        {
            List<AdPersonDto> personList = new List<AdPersonDto>();
            StringBuilder sb = new StringBuilder();
            int nameId = dataService.getIdOfAdPersonAttribute("cn");
            //Daten sammeln...
            bool first = true;
            foreach (DynamicDataTableRow item in currentSelectedPersonTableList)
            {
                personList.Add(resultListPerson[item.rowNumber]);
                if (first) first = false; else sb.Append(',');
                sb.Append(resultListPerson[item.rowNumber].dataDictionary[nameId]);
            }
            Clipboard.SetText(sb.ToString());
        }

        /// <summary>
        /// Erstellt einen Semikolonseparierten String aller Mail(mail)-Attribute der ausgewählten user
        /// und speichert das im Clipboard.
        /// </summary>
        /// <param name="param"></param>
        private async void onActionPersonEmailCopyClipboard(object param)
        {
            StringBuilder sb = new StringBuilder();
            int nameId = dataService.getIdOfAdPersonAttribute("mail");
            //Daten sammeln...
            bool first = true;
            foreach (DynamicDataTableRow item in currentSelectedPersonTableList)
            {
                if (first) first = false; else sb.Append(';');
                sb.Append(resultListPerson[item.rowNumber].dataDictionary[nameId]);
            }
            Clipboard.SetText(sb.ToString());
        }

        /// <summary>
        /// Erstellt einen Kommaseparierten String aller Name(cn)-Attribute der ausgewählten gruppen
        /// und speichert das im Clipboard.
        /// </summary>
        /// <param name="param"></param>
        private async void onActionGroupNameCopyClipboard(object param)
        {
            List<AdGroupDto> groupList = new List<AdGroupDto>();
            StringBuilder sb = new StringBuilder();
            int nameId = dataService.getIdOfAdGroupAttribute("cn");
            //Daten sammeln...
            bool first = true;
            foreach (DynamicDataTableRow item in currentSelectedGroupTableList)
            {
                groupList.Add(resultListGroup[item.rowNumber]);
                if (first) first = false; else sb.Append(',');
                sb.Append(resultListGroup[item.rowNumber].dataDictionary[nameId]);
            }
            Clipboard.SetText(sb.ToString());
        }

        private async void onActionPersonCopyClipboard(object param)
        {
            string commandParam = param as string;
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;
            bool firstItem2 = true;
            List<AdPersonDto> personList = new List<AdPersonDto>();
            //Daten sammeln...
            foreach (DynamicDataTableRow item in currentSelectedPersonTableList)
            {
                personList.Add(resultListPerson[item.rowNumber]);
            }
            int[] attributeIdList = new int[appSettings.getSearchResultUserAttributesList().Count];
            int i = 0;
            foreach(AdAttributeDto attributeItem in appSettings.getSearchResultUserAttributesList())
            {
                attributeIdList[i++] = attributeItem.id;
            }
            foreach(AdPersonDto personItem in personList) 
            { 

                if (firstItem)
                    firstItem = false;
                else
                    sb.Append(Environment.NewLine);

                firstItem2 = true;
                foreach (int id in attributeIdList)
                {
                    if (firstItem2)
                        firstItem2 = false;
                    else
                    {
                        if (commandParam.Equals(Constants.CommandParameterCopyAll))
                            sb.Append("\t");
                        else
                            sb.Append(",");
                    }
                    sb.Append(personItem.dataDictionary[id]);
                }
            }
            Clipboard.SetText(sb.ToString());
        }
        private bool canActionPersonCopyClipboard(object param)
        {
            return currentSelectedPersonTableList.Count > 0 ? true : false;
        }

        /// <summary>
        /// Speichert aus den Properties einer Person nur den Wert in Zwischenablage
        /// </summary>
        /// <param name="param"></param>
        private async void onActionPersonPropertiesCopyClipboard(object param)
        {
            StringBuilder sb = new StringBuilder();
            string paramValue = (string)param;
            bool firstItem = true;
            if (paramValue.Equals(Constants.CommandParameterCopyKeyValue))
            {
                foreach (object item in currentSelectedPersonsPropertiesList)
                {
                    if (firstItem) firstItem = false;
                    else sb.Append(Environment.NewLine);
                    sb.Append(((PersonPropertyListItem)item).displayName);
                    sb.Append("\t");
                    sb.Append(((PersonPropertyListItem)item).propertyValue);
                }
            } 
            else if (paramValue.Equals(Constants.CommandParameterCopyValueOnly))
            {
                foreach (object item in currentSelectedPersonsPropertiesList)
                {
                    if (firstItem) firstItem = false; 
                    else sb.Append(Environment.NewLine);
                    sb.Append(((PersonPropertyListItem)item).propertyValue);
                }
            }
            System.Windows.Clipboard.SetText(sb.ToString());
        }
        private bool canActionPersonPropertiesCopyClipboard(object param)
        {
            return currentSelectedPersonsPropertiesList.Count > 0 ? true : false;
        }

        /// <summary>
        /// Speichert die ausgewählten Gruppen einer Person in Zwischenablage.
        /// </summary>
        /// <param name="param"></param>
        private async void onActionPersonGroupsCopyClipboard(object param)
        {
            string commandParam = param as string;
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;

            if (commandParam.Equals(Constants.CommandParameterCopyAsCSV))
            {
                foreach (object item in currentSelectedPersonsGroupsList)
                {
                    if (firstItem) firstItem = false; else sb.Append(',');
                    sb.Append(((MemberListItem)item).shortName);
                }
            }
            else if (commandParam.Equals(Constants.CommandParameterCopyGroupDistinguishedOnly))
            {
                foreach (object item in currentSelectedPersonsGroupsList)
                {
                    if (firstItem) firstItem = false; else sb.Append(',');
                    sb.Append(((MemberListItem)item).distinguishedName);
                }
            }
            System.Windows.Clipboard.SetText(sb.ToString());
        }
        private bool canActionPersonGroupsCopyClipboard(object param)
        {
            return currentSelectedPersonsGroupsList.Count > 0 ? true : false;
        }



        

        private async void onActionGroupCopyClipboard(object param)
        {
            string paramValue = (string)param;
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;
            bool firstItem2 = true;
            List<AdGroupDto> groupList = new List<AdGroupDto>();

            //daten sammeln
            foreach (DynamicDataTableRow item in currentSelectedGroupTableList)
            {
                groupList.Add(resultListGroup[item.rowNumber]);
            }
            int[] attributeIdList = new int[appSettings.getSearchResultGroupAttributesList().Count];
            int i = 0;
            foreach (AdAttributeDto attributeItem in appSettings.getSearchResultGroupAttributesList())
            {
                attributeIdList[i++] = attributeItem.id;
            }
            foreach (AdGroupDto groupItem in groupList)
            {
                if (firstItem)
                    firstItem = false;
                else
                    sb.Append(Environment.NewLine);

                foreach (int id in attributeIdList)
                {
                    if (firstItem2)
                        firstItem2 = false;
                    else
                        sb.Append(";");
                    sb.Append(groupItem.dataDictionary[id]);
                }

                /*
                if (paramValue.Equals(Constants.CommandParameterCopyGroupNameOnly))
                {
                    foreach (object item in currentSelectedGroupList)
                    {
                        if (firstItem)
                            firstItem = false;
                        else
                            //sb.Append(Environment.NewLine);
                            sb.Append(',');
                        sb.Append(((DataTableGroupModel)item).name);
                    }
                }
                else if (paramValue.Equals(Constants.CommandParameterCopyGroupDistinguishedOnly))
                {
                    foreach (object item in currentSelectedGroupList)
                    {
                        if (firstItem)
                        {
                            sb.Append('\'');
                            firstItem = false;
                        }
                        else
                        {

                            sb.Append("\',\'");
                        }
                        //sb.Append(Environment.NewLine);
                        sb.Append(((DataTableGroupModel)item).distinguishedName);
                    }
                    sb.Append('\'');
                }
                else if (paramValue.Equals(Constants.CommandParameterCopyGroupAllProperties))
                {
                    foreach (object item in currentSelectedGroupList)
                    {
                        if (firstItem)
                            firstItem = false;
                        else
                            sb.Append(Environment.NewLine);
                        sb.Append(((DataTableGroupModel)item).name);
                        sb.Append('\t');
                        sb.Append(((DataTableGroupModel)item).distinguishedName);
                        sb.Append('\t');
                        sb.Append(((DataTableGroupModel)item).description);
                    }
                }
                */
                Clipboard.SetText(sb.ToString());
            }
        }
        internal bool canActionGroupCopyClipboard(object parameter)
        {
            return currentSelectedGroupTableList.Count > 0 ? true : false;
        }


        /// <summary>
        /// Kopiert die Daten aller Member einer Gruppe in die Zwischenablage.
        /// Hierbei handelt es sich um MemberListItem
        /// </summary>
        /// <param name="param"></param>
        private async void onActionGroupsMemberCopyClipboard(object param)
        {
            string commandParam = param as string;
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;
            bool multiLine = currentSelectedGroupsMemberList.Count > 1;
            string[,] dataTable; //row,col
            int rowIndex = 0;
            int colIndex = 0;
            

            if (commandParam.Equals(Constants.CommandParameterCopyAsCSV))
            {
                foreach (MemberListItem row in currentSelectedGroupsMemberList)
                {
                    if (firstItem) firstItem = false;
                    else
                        sb.Append(',');
                    sb.Append(row.shortName);
                }
                Clipboard.SetText(sb.ToString());
            }
            /*
            foreach (object item in currentSelectedGroupsMemberList)
            {
                if (commandParam.Equals(Constants.CommandParameterCopyAll))
                {
                    dataTable = new string[currentSelectedGroupsMemberList.Count, 2];
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append(Environment.NewLine);
                    sb.Append(((DataTablePersonModel)item).personalId);
                    sb.Append("\t");
                    sb.Append(((DataTablePersonModel)item).displayName);
                }
                else if (commandParam.Equals(Constants.CommandParameterCopyAsCSV))
                {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append(",");
                    sb.Append(((DataTablePersonModel)item).personalId);
                }

            }
            */
            //Clipboard.SetText(sb.ToString());
        }
        internal bool canActionGroupsMemberCopyClipboard(object parameter)
        {
            return currentSelectedGroupsMemberList.Count > 0 ? true : false;
        }

        private void showBusyMouse(bool busy)
        {
            if (busy)
            {
                //if(Mouse.OverrideCursor!=null && Mouse.OverrideCursor != Cursors.Wait)
                //    currentCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;
            }
            else
            {
                Mouse.OverrideCursor = currentCursor;
            }
        }

        private async void searchGroupAbortableAsync(AdSearchSettingsDto searchSettings)
        {
            try
            {
                showBusyMouse(true);
                searchCancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = searchCancellationTokenSource.Token;
                groupSearchResultTableList.Clear();
                groupMemberList.Clear();
                AdSearchResultDto<List<AdGroupDto>> result = null;
                
            }
            catch (Exception)
            { }//cancel search
            showBusyMouse(false);
        }


        private async void searchPersonAbortableAsync(AdSearchSettingsDto searchSettings)
        {
            try
            {
                showBusyMouse(true);
                searchCancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = searchCancellationTokenSource.Token;
                
                //search
                personSearchResultTableList.Clear();
                personPropertyList.Clear();
                personGroupMemberList.Clear();

                
            }
            catch (Exception)
            {//cancel search
            }
            showBusyMouse(false);
        }
        internal bool canActionSearch(object parameter)
        {
            return commandSearchEnabled;
        }

        internal bool canActionCancelSearch(object parameter)
        {
            return true;
        }
        private async void onActionCancelSearch(object param)
        {
            if (searchCancellationTokenSource != null)
                searchCancellationTokenSource.Cancel();
        }

        internal bool canActionClearSearchfields(object parameter)
        {
            return true;
        }
        private async void onActionClearSearchfields(object param)
        {
            foreach(SearchfieldListItem item in searchPersonFieldList)
            {
                item.textFieldValue = "";
            }
            

            OnPropertyChanged("searchPersonFieldList");
        }

        /// <summary>
        /// Search person
        /// </summary>
        private async void onActionSearch(object param)
        {
            string? actionParam = param as string;
            //search person
            if (actionParam!=null && actionParam.Equals(Constants.CommandParameterPerson))
            {
                
            }
            // search group
            if (actionParam != null && actionParam.Equals(Constants.CommandParameterGroup))
            {
                commandSearchEnabled = false;
                AdSearchSettingsDto searchSettings = getSearchGroupParameters();
                //check
                //todo checkSearchGroupSettings neu schreiben
                //SearchSettingsCheckResultDto checkResult = dataService.checkSearchGroupSettings(searchSettings);
                //if (checkResult.isValid)
                if (true)
                {
                    //call dispatcher for startSearchGroup
                    Dispatcher.CurrentDispatcher.Invoke(
                          DispatcherPriority.Background,
                          new Action(() => {
                              searchGroupAbortableAsync(searchSettings);
                          }));
                }
                
                commandSearchEnabled = true;
            }
        }


        /// <summary>
        /// Erstellt aus der Liste der ErrorCodes einen String mit allen Fehlermeldungen als Textform in der eingestellten Sprache.
        /// </summary>
        /// <param name="errorCodeList"></param>
        /// <returns></returns>
        private string getSearchSettingsCheckResultMessages(List<ESearchSettingsErrorCode> errorCodeList)
        {
            StringBuilder sb = new StringBuilder();
            foreach(ESearchSettingsErrorCode item in errorCodeList)
            {
                if (item == ESearchSettingsErrorCode.AllFieldsEmpty)
                    sb.Append(string.Format("* {0}", Resources.M002_AllFieldsEmpty));
                if (item == ESearchSettingsErrorCode.BeginWithAsterisk)
                    sb.Append(string.Format("\n* {0}", Resources.M003_BeginWithAsterisk));
                if (item == ESearchSettingsErrorCode.ContainsAsterisk)
                    sb.Append(string.Format("\n* {0}", Resources.M004_ContainsAsterisk));
                if (item == ESearchSettingsErrorCode.ContainsDelimiter)
                    sb.Append(string.Format("\n* {0}", Resources.M005_ContainsDelimiter));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Liest die Suchfelder für Personen aus und liefert ein SearchSettings-Objekt.
        /// </summary>
        /// <returns></returns>
        private AdSearchSettingsDto getSearchPersonParameters()
        {
            Dictionary<int, string[]> searchValues = new Dictionary<int, string[]>();//Attribute-ID,Value-String-Array
            foreach (SearchfieldListItem item in searchPersonFieldList)
            {
                string cleanedValue = item.textFieldValue != null ? item.textFieldValue.Trim().Replace(';', ',').Replace(" ", string.Empty) :string.Empty;
                string[] fieldData = cleanedValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (fieldData != null && fieldData.Length > 0)
                    searchValues.Add(item.adAttributeId, fieldData);
            }
            AdSearchSettingsDto result = new AdSearchSettingsDto(ESearchType.USER, searchValues);
            return result;
        }

        /// <summary>
        /// Liest die Suchfelder für Gruppen aus und liefert ein SearchSettings-Objekt.
        /// </summary>
        /// <returns></returns>
        private AdSearchSettingsDto getSearchGroupParameters()
        {
            Dictionary<int, string[]> searchValues = new Dictionary<int, string[]>();//Attribute-ID,Value-String-Array
            foreach (SearchfieldListItem item in searchGroupFieldList)
            {
                string cleanedValue = item.textFieldValue != null ? item.textFieldValue.Trim().Replace(';', ',').Replace(" ", string.Empty) : string.Empty;
                string[] fieldData = cleanedValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (fieldData != null && fieldData.Length > 0)
                    searchValues.Add(item.adAttributeId, fieldData);
            }
            AdSearchSettingsDto result = new AdSearchSettingsDto(ESearchType.GROUP, searchValues);
            return result;
        }

        /// <summary>
        /// Prüft, ob die Suchfelder korrekte Eingaben haben.
        /// 
        /// </summary>
        /// <param name="searchSettings"></param>
        /// <returns></returns>
        private SearchSettingsCheckResultDto checkSearchSettings(AdSearchSettingsDto searchSettings)
        {
            SearchSettingsCheckResultDto result = new SearchSettingsCheckResultDto();
            result.isValid = true;
            bool allFieldsEmpty = true;
            StringBuilder errorMsg = new StringBuilder();
            if(searchSettings.searchData.Count==0)
                allFieldsEmpty = true;
            foreach (int adId in searchSettings.searchData.Keys)
            {
                string[] valueItem = searchSettings.searchData[adId];
                if (valueItem.Length > 0)
                {
                    allFieldsEmpty=false;
                    /*
                    foreach (string field in valueItem) 
                    {
                        if (field.StartsWith('*'))
                        {
                            result.errorCodeList.Add(ESearchSettingsErrorCode.BeginWithAsterisk);
                            result.isValid = false;
                        }
                    }
                    */
                }
            }
            if (allFieldsEmpty)
            {
                result.isValid = false;
                result.errorCodeList.Add(ESearchSettingsErrorCode.AllFieldsEmpty);
            }
            return result;
        }

        /// <summary>
        /// Wird beim Schließen der MainApp aufgerufen.
        /// </summary>
        //public void onCloseWindow()
        //{

        //}

        /// <summary>
        /// Wird beim X-Close-Klick als zweites aufgerufen.
        /// Wird aus xaml.cs aufgerufen, wenn Benutzer auf Window-Button X klickt.
        /// Hier im Model wird entschieden, ob dem Close-Wunsch zugestimmt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (appSettings.isUnsafedData())
            {
                int btn = -1;
                CustomMessageBoxParameter parameter = new CustomMessageBoxParameter
                {
                    title = Resources.L071_UnsafedData,
                    messageType = CustomMessageBoxParameter.MessageTypExclamationRed,
                };
                parameter.messageText = string.Format("{0}\n", Resources.M010_UnsafedData);
                parameter.windowWidth = 450;
                parameter.windowHeight = 200;
                parameter.buttonList = new CustomMessageBoxButtonDescriptor[2];
                parameter.buttonList[0] = new CustomMessageBoxButtonDescriptor(Resources.L074_OK, true);
                parameter.buttonList[0].isDefaultKeystroke = true;
                parameter.buttonList[1] = new CustomMessageBoxButtonDescriptor(Resources.L040_Cancel, true);
                parameter.buttonList[1].isCancelKeystroke = true;
                parameter.buttonList[0].buttonActionCommand = (x) => { btn = 0; };
                parameter.buttonList[1].buttonActionCommand = (x) => { btn = 1; };

                viewManager.showMesgBoxError(parameter);
                if (btn == 1)
                {
                    e.Cancel = true;// false->zustimmen
                    return;
                }
            }
            viewManager.sendAllWindowsCloseEvent();
            e.Cancel = false;// false->zustimmen
        }
    }
}
