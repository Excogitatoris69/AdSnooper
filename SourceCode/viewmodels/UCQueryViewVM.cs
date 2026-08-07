using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using AdSnooperGui.UserControls;
using Antlr4.Runtime.Atn;
using CoreAdSnooper.interfaces;
using DomainAdSnooper.dto;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AdSnooperGui.viewmodels
{

    public class UCQueryViewVM: SqlQueryBasicVM
    {
        public ObservableCollection<TabItem> editorTabList { get; set; }

        private int _selectedEditorTabIndex;
        public int selectedEditorTabIndex
        {
            get { return _selectedEditorTabIndex; }
            set
            {
                _selectedEditorTabIndex = value;
                OnPropertyChanged(nameof(selectedEditorTabIndex));
            }
        }

        private ReaderWriterLockSlim runningBatchListLockSlim = null;
        private Dictionary<string, QueryBatchStatusDto> runningBatchList = null;

        private List<QueryBatchFileDto> _queryBatchFileDtoList;
        private List<string> currentEditList = null;//alle queryname, die gerade im editor offen sind
        //private List<string> currentRunningList = null;//alle queryname, die gerade im run-modus sind sind

        public ObservableCollection<string> queryBatchFileItemList { get; set; }

        //Index der Liste mit Batch-Queries
        private int _selectedQueryBatchFileItemListIndex;
        public int selectedQueryBatchFileItemListIndex
        {
            get { return _selectedQueryBatchFileItemListIndex; }
            set
            {
                _selectedQueryBatchFileItemListIndex = value;
                OnPropertyChanged(nameof(selectedQueryBatchFileItemListIndex));
            }
        }

        


        //private IDataService dataService=null;
        private ViewManager _viewManager = null;
        private List<SqlEditorVM> _sqlEditorTabContentList = null;
        private int tabCounter = 1;

        public GenericCommand cmdEdit { get; set; }
        public GenericCommand cmdDelete { get; set; }
        public GenericCommand cmdRun { get; set; }
        public GenericCommand cmdCancel { get; set; }
        public GenericCommand cmdAddNew { get; set; }
        

        public UCQueryViewVM(IDataService dataService, ViewManager viewManager)
        {
            this.dataService = dataService;
            this._viewManager = viewManager;
            editorTabList = new ObservableCollection<TabItem>();
            _sqlEditorTabContentList = new List<SqlEditorVM>();
            currentEditList = new List<string>();
            runningBatchList = new Dictionary<string, QueryBatchStatusDto>();
            runningBatchListLockSlim = new ReaderWriterLockSlim();
            init();
        }

        private void init()
        {
            queryBatchFileItemList = new ObservableCollection<string>();
            
            //Plus-Tab
            //TabItem addButtonTabItem = new TabItem();
            //addButtonTabItem.Header = "+";
            //addButtonTabItem.Content = new Label { Content = "+" };
            //addButtonTabItem.DataContext = this;
            //editorTabList.Add(addButtonTabItem);
            //addNewTabItem();

            //Commands
            cmdEdit = new GenericCommand(onActionEdit, canActionEdit);
            cmdDelete = new GenericCommand(onActionDelete, canActionDelete);
            cmdRun = new GenericCommand(onActionRun, canActionRun);
            cmdCancel = new GenericCommand(onActionCancel, canActionCancel);
            cmdAddNew = new GenericCommand(onActionAddNew, canActionAddNew);
            
            readQueryfileData();
        }

        




        //private void onTabCloseEvent(object? sender, EventArgs e)
        //{
        //    if (editorTabList.Count <= 2) 
        //        return;//es muss immer ein Tab bleiben sowie der Plus-Tab
        //    int foundIndex = editorTabList.IndexOf((TabItem)sender);
        //    if(foundIndex >= 0)
        //    {
        //        if(selectedEditorTabIndex > 0)
        //            selectedEditorTabIndex = selectedEditorTabIndex - 1;//zuvor den vorherigen selektieren
        //        editorTabList.RemoveAt(foundIndex);
        //    }
        //}


        /// <summary>
        /// Wird aufgerufen, wenn ein Tabwechsel stattfindet.
        /// Ist es der Plus-Tab, wird ein neuer Tab eröffnet.
        /// </summary>
        /// <param name="index"></param>
        public void editorTabControlSelectionChanged(int index)
        {
            //if (index == editorTabList.Count - 1 && editorTabList.Count > 1)//wenn Plus-Tab
            //{
            //    addNewTabItem();
            //    selectedEditorTabIndex = index ;//neuen selectieren
            //}
        }


        /// <summary>
        /// Erzeugt und öffnet einen neuen Tab.
        /// </summary>
        public void addNewTabItem()
        {
            //SqlEditorTabItem tabItem = new SqlEditorTabItem();
            TabItem tabItem = new TabItem();
            SqlEditorVM sqlEditorVM = new SqlEditorVM(dataService, _viewManager);
            sqlEditorVM.buttonEvent += onSqlEditorButtonEvent;
            UCSqlEditor sqlEditorView = new UCSqlEditor(sqlEditorVM);
            tabItem.Content = sqlEditorView;
            //tabItem.closeTabEvent += onTabCloseEvent;

            string queryTabTitle = "New " + tabCounter;
            //tabItem.HeaderText = queryName;
            tabItem.Header = queryTabTitle;
            sqlEditorVM.queryTabTitle = queryTabTitle;

            _sqlEditorTabContentList.Add(sqlEditorVM);

            //editorTabList.Insert(editorTabList.Count-1, tabItem); //  vor dem Plus-Tab einfügen
            editorTabList.Insert(editorTabList.Count, tabItem); //  vor dem Plus-Tab einfügen
            tabCounter++;
            //sqlEditorVM.avalonEditUploadText = "/*\r\nMy first query to search for people: Query-1\r\n*/\r\nselect PersonalId, Salutation, Firstname, Lastname, Department, Description,\r\nActive, Displayname, Email, Telephone, Mobile, Fax, \r\nStreetaddress, Postalcode, Location, CompanyLong, Company, Roomnumber, Countrycode, group \r\n\r\nwhere \r\n\tPersonalId='12345' \r\n# and Department='1234'\r\n;\r\n\r\n/*\r\nexportsettings ( \r\n\tfilepath='c:\\temp\\adsnooper_query-1_export.csv' ,\r\n\theader=true ,\r\n\tdelimiter=Semicolon   \r\n);\r\n*/\r\n";
            sqlEditorVM.avalonEditUploadText = "# My first query to search for person\r\nselect *\r\nfrom persons\r\nwhere name='p*';\r\n\r\n/*\r\n#My first query to search for group\r\nselect *\r\nfrom groups\r\nwhere name='Grp*';\r\n*/\r\n\r\n/*\r\nexportsettings ( \r\n\tfilepath='c:\\adsnooper_query-1_export.csv' ,\r\n\theader=true ,\r\n\tdelimiter=Semicolon   \r\n);\r\n*/\r\n\r\n";
            sqlEditorVM.isUnsafedChanges = false;
        }

        public bool isQueryEditMode(string queryname)
        {
            return currentEditList.Contains(queryname);
        }

        /// <summary>
        /// Wenn ein Listeneintrag in der Batchliste selektiert weird.
        /// </summary>
        public void onBatchListSelectionChanged()
        {
            //buttons
            if (queryBatchFileItemList.Count == 0) return;
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            QueryBatchFileDto foundQueryBatchFileDto = null;
            foreach (QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                if (item.queryName.Equals(selectedQueryName))
                {
                    foundQueryBatchFileDto = item;
                    break;
                }
            }
            if (currentEditList.Contains(foundQueryBatchFileDto.queryName))//ist im editor offen
            {

            }
                

        }

        
        /// <summary>
        /// Wenn eine Querydatei im Editor angezeigt werden soll
        /// </summary>
        public void openTabItem()
        {
            if (queryBatchFileItemList.Count == 0) return;
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            QueryBatchFileDto foundQueryBatchFileDto = null;
            foreach (QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                if (item.queryName.Equals(selectedQueryName))
                {
                    foundQueryBatchFileDto = item;
                    break;
                }
            }
            if (currentEditList.Contains(foundQueryBatchFileDto.queryName))//verhindert doppelt offene Tabreiter
                return;
            currentEditList.Add(foundQueryBatchFileDto.queryName);

            TabItem tabItem = new TabItem();
            //tabItem.Template = new ControlTemplate()

            SqlEditorVM sqlEditorVM = new SqlEditorVM(dataService, _viewManager);
            sqlEditorVM.buttonEvent += onSqlEditorButtonEvent;
            UCSqlEditor sqlEditorView = new UCSqlEditor(sqlEditorVM);
            tabItem.Content = sqlEditorView;

            tabItem.Header = foundQueryBatchFileDto.queryName;
            sqlEditorVM.queryTabTitle = foundQueryBatchFileDto.queryName;
            sqlEditorVM.queryName = foundQueryBatchFileDto.queryName;
            sqlEditorVM.avalonEditUploadText = foundQueryBatchFileDto.queryBatchData.queryText;
            sqlEditorVM.isUnsafedChanges = false;
            _sqlEditorTabContentList.Add(sqlEditorVM);
            //editorTabList.Insert(editorTabList.Count - 1, tabItem); //  vor dem Plus-Tab einfügen
            editorTabList.Insert(editorTabList.Count, tabItem); //  vor dem Plus-Tab einfügen
            selectedEditorTabIndex = editorTabList.Count - 1;//neuen selectieren
        }

        //=================================================
        // private
        //=================================================

        private void closeTabItem(string queryTitle)
        {
            TabItem selectedSqlEditorTabItem = getSqlEditorTabItemByName(queryTitle);
            //if (editorTabList.Count <= 2)
            //    return;//es muss immer ein Tab bleiben sowie der Plus-Tab

            //check unsafedChanges
            SqlEditorVM foundSqlEditorVM = null;
            int foundIndex = -1;
            foreach (SqlEditorVM item in _sqlEditorTabContentList)
            {
                foundIndex++;
                if (item.queryTabTitle.Equals(queryTitle))
                {
                    foundSqlEditorVM = item;
                    break;
                }
            }
            if (foundSqlEditorVM.isUnsafedChanges)
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
                
                _viewManager.showMesgBoxError(parameter);
                if (btn == 1)
                {
                    return;
                }
                foundSqlEditorVM.isUnsafedChanges = false;//ignore

            }
            currentEditList.Remove(foundSqlEditorVM.queryName);
            foundSqlEditorVM.buttonEvent -= onSqlEditorButtonEvent;
            if (foundIndex >= 0)
            {
                _sqlEditorTabContentList.RemoveAt(foundIndex);
            }
            selectedSqlEditorTabItem.Content = null;
            foundIndex = editorTabList.IndexOf(selectedSqlEditorTabItem);
            if (foundIndex >= 0)
            {
                if (selectedEditorTabIndex > 0)
                    selectedEditorTabIndex = selectedEditorTabIndex - 1;//zuvor den vorherigen selektieren
                editorTabList.RemoveAt(foundIndex);
            }
            selectedSqlEditorTabItem = null;

        }

        private SqlEditorVM getSqlEditorViewModelByQueryTitle(string queryTitle)
        {
            SqlEditorVM foundSqlEditorVM = null;
            foreach (SqlEditorVM item in _sqlEditorTabContentList)
            {
                if (item.queryTabTitle.Equals(queryTitle))
                {
                    foundSqlEditorVM = item;
                    break;
                }
            }
            return foundSqlEditorVM;
        }

        //private SqlParserResultDto refreshSqlParsingResult(SqlEditorVM sqlEditorVM)
        //{
        //    SqlParserResultDto currentPasingResult = dataService.parseSqlQueryString(sqlEditorVM.editorText);
        //    StringBuilder sbErrorText = new StringBuilder();
        //    foreach(SqlParserSyntaxErrorDto item in currentPasingResult.syntaxErrorList)
        //    {
        //        sbErrorText.Append(item.ToString());
        //        sbErrorText.Append("\n");
        //    }
        //    sqlEditorVM.sqlParseSyntaxErrorText = sbErrorText.ToString();
        //    if (currentPasingResult.syntaxErrorList.Count == 0)
        //    {
        //        sqlEditorVM.sqlParsedSyntaxValid = true;
        //        sqlEditorVM.sqlParsedAdQueryString = currentPasingResult.adQueryString;
        //    }
        //    else
        //    {
        //        sqlEditorVM.sqlParsedAdQueryString = "";
        //        sqlEditorVM.sqlParsedSyntaxValid = false;
        //    }
        //    return currentPasingResult;
        //}


        /// <summary>
        /// Speichert eine Query.
        /// Entweder ist sie neu und muss neu angelegt werden oder sie ist nicht neu und die Daten müssen nur geändert werden.
        /// Wenn saveAs=true, wird immer ein Name erfragt. Ist es false, wird nur dann ein Name erfragt, wenn es noch keinen gibt.
        /// Der eingegebene Name ist nur ein Wunsch. Der wirklich verwendete Namen muss erst ermittelt werden.
        /// Die Namensernmittlung findet anhand des Dateisystems statt. Dort wird geprüft, ob der Name bereits vergeben ist.
        /// In diesem Fall wird hinter den Wunschnamen ein Prefix '(x)' angehängt, wobei x eine laufende Nummer ist.
        /// </summary>
        /// <param name="queryTitle"></param>
        /// <param name="saveAs"></param>
        private void saveQueryAsBatchfile(string queryTitle, bool saveAs)
        {
            //querydaten holen. wenn nicht da, dann neu anlegen
            TabItem selectedSqlEditorTabItem = getSqlEditorTabItemByName(queryTitle);
            SqlEditorVM sqlEditorVM = ((UCSqlEditor)selectedSqlEditorTabItem.Content).viewmodel;
            SqlParserResultDto aSqlParserResultDto = refreshSqlParsingResult(sqlEditorVM);
            bool hasUserCanceled = false;
            string desiredName = null;//wunschname


            //Namen erfragen, wenn gewünscht oder notwendig, weil die Query noch keinen Namen hat, sondern bisher nur einen Titel.
            if (saveAs || string.IsNullOrEmpty(sqlEditorVM.queryName))
            {
                //messagebox anzeigen um Namen zu erfragen
                CustomMessageBoxParameter parameter = new CustomMessageBoxParameter
                {
                    title = Resources.L072_NewQuery,
                    messageType = CustomMessageBoxParameter.MessageTypQuestion,
                };
                parameter.messageText = string.Format("{0}\n", Resources.M011_InputNewQueryName); 
                parameter.inputText = sqlEditorVM.queryName!=null? sqlEditorVM.queryName:queryTitle;
                parameter.windowWidth = 450;
                parameter.windowHeight = 200;
                parameter.buttonList = new CustomMessageBoxButtonDescriptor[2];
                parameter.buttonList[0] = new CustomMessageBoxButtonDescriptor(Resources.L040_Cancel, true);
                parameter.buttonList[0].isDefaultKeystroke = false;
                parameter.buttonList[0].isCancelKeystroke = true;
                parameter.buttonList[1] = new CustomMessageBoxButtonDescriptor(Resources.L041_Save, true);
                parameter.buttonList[1].isDefaultKeystroke = true;
                int btn = -1;
                parameter.buttonList[0].buttonActionCommand = (x) => { btn = 0; };
                parameter.buttonList[1].buttonActionCommand = (x) => { btn = 1; };

                bool isInputValid = false;
                while (!isInputValid)
                {
                    _viewManager.showInputBox(parameter);
                    if (btn == 1)
                    {
                        desiredName = parameter.inputText.Trim();
                        if (!string.IsNullOrEmpty(desiredName))
                            isInputValid = true;
                    }
                    else
                    {
                        hasUserCanceled = true;
                        isInputValid = true;
                    }
                }
            }

            // Es geht nur weiter, wenn der user nicht abgebrochen hat.
            if (!hasUserCanceled)
            {
                //Query hat bereits einen Namen und User will die Query unter neuem Namen speichert -> SavaAs
                if(sqlEditorVM.queryName != null && saveAs)
                {
                    QueryBatchFileDto currentQueryBatchFileDto = getQueryBatchFileDto(sqlEditorVM.queryName);
                    currentQueryBatchFileDto.fileName = null;
                    currentQueryBatchFileDto.queryName = desiredName;
                    QueryBatchDto queryBatchData = currentQueryBatchFileDto.queryBatchData;
                    queryBatchData.queryText = sqlEditorVM.editorText;
                    if (aSqlParserResultDto.exportSettings != null)
                    {
                        queryBatchData.exportFilePath = aSqlParserResultDto.exportSettings.exportFilePath;
                        queryBatchData.exportDelimiter = aSqlParserResultDto.exportSettings.delimiter;
                        queryBatchData.isExportHeader = aSqlParserResultDto.exportSettings.isHeader;
                    }
                    else
                    {
                        queryBatchData.exportFilePath = null;
                        queryBatchData.exportDelimiter = EDelimiter.Semicolon;
                        queryBatchData.isExportHeader = false;
                    }
                    dataService.writeQueryFile(currentQueryBatchFileDto);
                    currentEditList.Remove(sqlEditorVM.queryName);
                    //Hier erhalten wir jetzt den verwendeten Namen der Query. Dieser wird automatisch wenn notwendig angepasst.
                    sqlEditorVM.queryName = currentQueryBatchFileDto.queryName;
                    sqlEditorVM.queryTabTitle = currentQueryBatchFileDto.queryName;
                    currentEditList.Add(sqlEditorVM.queryName);
                    selectedSqlEditorTabItem.Header = sqlEditorVM.queryName;
                    sqlEditorVM.isUnsafedChanges = false;
                    readQueryfileData();
                }
                //Query hat keinen Namen -> SavaAs
                else if (sqlEditorVM.queryName == null)
                {
                    QueryBatchFileDto newQueryBatchFileDto = new QueryBatchFileDto();
                    newQueryBatchFileDto.fileName = null;
                    newQueryBatchFileDto.queryName = desiredName;
                    newQueryBatchFileDto.queryBatchData = new QueryBatchDto();
                    newQueryBatchFileDto.queryBatchData.queryText = sqlEditorVM.editorText;
                    if (aSqlParserResultDto.exportSettings != null)
                    {
                        newQueryBatchFileDto.queryBatchData.exportFilePath = aSqlParserResultDto.exportSettings.exportFilePath;
                        newQueryBatchFileDto.queryBatchData.exportDelimiter = aSqlParserResultDto.exportSettings.delimiter;
                        newQueryBatchFileDto.queryBatchData.isExportHeader = aSqlParserResultDto.exportSettings.isHeader;
                    }
                    else
                    {
                        newQueryBatchFileDto.queryBatchData.exportFilePath = null;
                        newQueryBatchFileDto.queryBatchData.exportDelimiter = EDelimiter.Semicolon;
                        newQueryBatchFileDto.queryBatchData.isExportHeader = false;
                    }
                    dataService.writeQueryFile(newQueryBatchFileDto);
                    //Hier erhalten wir jetzt den verwendeten Namen der Query. Dieser wird automatisch wenn notwendig angepasst.
                    sqlEditorVM.queryName = newQueryBatchFileDto.queryName;
                    sqlEditorVM.queryTabTitle = newQueryBatchFileDto.queryName;
                    currentEditList.Add(sqlEditorVM.queryName);
                    selectedSqlEditorTabItem.Header = sqlEditorVM.queryName;
                    sqlEditorVM.isUnsafedChanges = false;
                    readQueryfileData();
                }
                //Query hat bereits einen Namen und User will die Query nur speichern
                else if (sqlEditorVM.queryName != null && !saveAs)
                {
                    QueryBatchFileDto currentQueryBatchFileDto = getQueryBatchFileDto(sqlEditorVM.queryName);
                    QueryBatchDto queryBatchData = currentQueryBatchFileDto.queryBatchData;
                    queryBatchData.queryText = sqlEditorVM.editorText;
                    if (aSqlParserResultDto.exportSettings != null)
                    {
                        queryBatchData.exportFilePath = aSqlParserResultDto.exportSettings.exportFilePath;
                        queryBatchData.exportDelimiter = aSqlParserResultDto.exportSettings.delimiter;
                        queryBatchData.isExportHeader = aSqlParserResultDto.exportSettings.isHeader;
                    }
                    else
                    {
                        queryBatchData.exportFilePath = null;
                        queryBatchData.exportDelimiter = EDelimiter.Semicolon;
                        queryBatchData.isExportHeader = false;
                    }
                        dataService.writeQueryFile(currentQueryBatchFileDto);
                    sqlEditorVM.isUnsafedChanges = false;
                    readQueryfileData();
                }

            }

        }
                

        private TabItem getSqlEditorTabItemByName(string searchQueryName)
        {
            TabItem foundSqlEditorTabItem = null;
            SqlEditorVM foundSqlEditorVM = null;
            for (int x = 0; x < editorTabList.Count ; x++)
            { //letzer Tab ist PlusTab
                //SqlEditorTabItem aSqlEditorTabItem = (SqlEditorTabItem)editorTabList[x];
                TabItem aSqlEditorTabItem = (TabItem)editorTabList[x];
                foundSqlEditorVM = ((UCSqlEditor)aSqlEditorTabItem.Content).viewmodel;
                if (foundSqlEditorVM.queryTabTitle.Equals(searchQueryName))
                {
                    foundSqlEditorTabItem = aSqlEditorTabItem;
                    break;
                }
            }
            return foundSqlEditorTabItem;
        }


        private void readQueryfileData()
        {
            if (_queryBatchFileDtoList != null)
                _queryBatchFileDtoList.Clear();
            _queryBatchFileDtoList = dataService.getQueryFileList();
            queryBatchFileItemList.Clear();
            foreach(QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                queryBatchFileItemList.Add(item.queryName);
            }
            _selectedQueryBatchFileItemListIndex = 0;
        }


        /// <summary>
        /// Sucht und liefert in der Liste aller geladenen Batchfiles nach dem QueryName
        /// </summary>
        /// <param name="searchQueryName"></param>
        /// <returns></returns>
        private QueryBatchFileDto getQueryBatchFileDto(string searchQueryName)
        {
            if(string.IsNullOrEmpty(searchQueryName)) return null;
            QueryBatchFileDto foundQueryBatchFileDto = null;
            foreach (QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                if (item.queryName.Equals(searchQueryName))
                {
                    foundQueryBatchFileDto = item;
                    break;
                }
            }
            return foundQueryBatchFileDto;
        }



        private void onSqlEditorButtonEvent(object? sender, SqlEditorButtonEventArgs e)
        {
            if(e.command == ECommandButtonId.SAVE)
            {
                saveQueryAsBatchfile(e.queryTitle, false);
            }
            if (e.command == ECommandButtonId.SAVEAS)
            {
                saveQueryAsBatchfile(e.queryTitle, true);
            }
            //if (e.command == ECommandButtonId.RUN)
            //{
            //    runQuery(e.queryTitle);
            //}
            if (e.command == ECommandButtonId.CLOSETAB)
            {
                closeTabItem(e.queryTitle);
            }

        }



        #region Commands
        private bool canActionCancel(object arg)
        {
            bool result = false;
            if (queryBatchFileItemList.Count == 0) return false;
            //aktuell selektierten BatchFile besorgen
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            runningBatchListLockSlim.EnterReadLock();
            if (runningBatchList.ContainsKey(selectedQueryName))
            {
                result = true;
            }
            runningBatchListLockSlim.ExitReadLock();
            return result;
        }

        private void onActionCancel(object obj)
        {
            //aktuell selektierten BatchFile besorgen
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            QueryBatchStatusDto aQueryBatchStatusDto = null;
            runningBatchListLockSlim.EnterWriteLock();
            if (runningBatchList.ContainsKey(selectedQueryName))
            {
                if (runningBatchList.TryGetValue(selectedQueryName, out aQueryBatchStatusDto))
                    aQueryBatchStatusDto.cancellationTokenSource.Cancel();
            }
            runningBatchListLockSlim.ExitWriteLock();
        }

        private bool canActionRun(object arg)
        {
            if (queryBatchFileItemList.Count == 0) return false;
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            
            //check unsafedDate
            bool unsagedChanges = false;
            SqlEditorVM aSqlEditorVM = getSqlEditorViewModelByQueryTitle(selectedQueryName);
            if (aSqlEditorVM != null && aSqlEditorVM.isUnsafedChanges)
                unsagedChanges = true;

            //check running status
            QueryBatchFileDto selectedQueryBatchFileDto = getQueryBatchFileDto(selectedQueryName);
            bool currentRunning = false;
            runningBatchListLockSlim.EnterReadLock();
            if (runningBatchList.ContainsKey(selectedQueryName))
            {
                currentRunning = true;
            }
            runningBatchListLockSlim.ExitReadLock();


            return
                //!currentEditList.Contains(selectedQueryName) //nicht im editor
                !unsagedChanges //keine ungespeicherten Daten im Editor
                && !string.IsNullOrEmpty(selectedQueryBatchFileDto.queryBatchData.exportFilePath) //nur wenn exportsettings
                && !currentRunning //wenn er nicht läuft
                ;
        }

        /// <summary>
        /// Im Batchbetrieb unterscheidet sich das Run-Kommando vom SqlEditor-Run.
        /// Hier werden die Daten sofort in einer Datei gespeichert.
        /// </summary>
        /// <param name="obj"></param>
        private void onActionRun(object obj)
        {
            
            if (queryBatchFileItemList.Count == 0) return;
            //aktuell selektierten BatchFile besorgen
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            QueryBatchFileDto foundQueryBatchFileDto = null;
            foreach (QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                if (item.queryName.Equals(selectedQueryName))
                {
                    foundQueryBatchFileDto = item;
                    break;
                }
            }
            if (foundQueryBatchFileDto == null) return;

            QueryBatchDto aQueryBatchDto = foundQueryBatchFileDto.queryBatchData;
            if (!string.IsNullOrEmpty(aQueryBatchDto.exportFilePath))
            {
                //Daten aus Querytext parsen und BatchRun vorbereiten
                SqlParserResultDto aSqlParserResultDto = dataService.parseSqlQueryString(aQueryBatchDto.queryText);
                if (aSqlParserResultDto.syntaxErrorList.Count > 0) return;//todo Fehler in Logdatei schreiben

                //query settings vorbereiten
                ExecuteQuerySettingsDto aExecuteQuerySettingsDto = new ExecuteQuerySettingsDto();
                aExecuteQuerySettingsDto.searchType = ESearchType.USER;
                aExecuteQuerySettingsDto.adQueryString = aSqlParserResultDto.adQueryString;
                if (aSqlParserResultDto.orderByField != null)
                    aExecuteQuerySettingsDto.orderbyField = aSqlParserResultDto.orderByField.adFieldname;
                List<string> outputAdFieldList = new List<string>();
                foreach (SqlAdFieldnameDescriptor item in aSqlParserResultDto.selectedFieldList)
                {
                    outputAdFieldList.Add(item.adFieldname);
                }
                aExecuteQuerySettingsDto.propertyOutputList.AddRange(outputAdFieldList);

                //query ausführen in thread
                QueryBatchStatusDto aQueryBatchStatusDto = null;
                runningBatchListLockSlim.EnterWriteLock();
                if (!runningBatchList.ContainsKey(selectedQueryName))
                {
                    aQueryBatchStatusDto = new QueryBatchStatusDto();
                    aQueryBatchStatusDto.cancellationTokenSource = new CancellationTokenSource();
                    runningBatchList.Add(selectedQueryName, aQueryBatchStatusDto);
                }
                runningBatchListLockSlim.ExitWriteLock();
                Task batchTask = Task.Factory.StartNew(async () =>
                {
                    //Thread.Sleep(10000);
                    AdQueryEcecuteResultDto result = dataService.executeSqlQuery(aExecuteQuerySettingsDto);
                    if (result.isSuccessful && result.dataType == typeof(List<AdPersonDto>))
                    {
                        List<AdPersonDto> dataList = result.resultData as List<AdPersonDto>;
                        if (dataList.Count > 0)
                        {
                            ExportSettingsDto aExportSettingsDto = new ExportSettingsDto();
                            aExportSettingsDto.destination = EExportDestination.File;
                            aExportSettingsDto.filePathPerson = aQueryBatchDto.exportFilePath;
                            aExportSettingsDto.delimiter = aQueryBatchDto.exportDelimiter;
                            aExportSettingsDto.isHeader = aQueryBatchDto.isExportHeader;
                            aExportSettingsDto.headerList = new string[aSqlParserResultDto.selectedFieldList.Count];
                            int headerListIndex = 0;
                            foreach (SqlAdFieldnameDescriptor aSqlAdFieldnameDescriptorItem in aSqlParserResultDto.selectedFieldList)
                            {
                                aExportSettingsDto.headerList[headerListIndex++] = aSqlAdFieldnameDescriptorItem.sqlFieldname;
                            }
                            aExportSettingsDto.selectedFieldList = aSqlParserResultDto.selectedFieldList;

                            //todo onActionRun korrigieren
                            //dataService.exportSqlPersonResultList(aExportSettingsDto, dataList);
                        }
                    }
                }, aQueryBatchStatusDto.cancellationTokenSource.Token);


                batchTask.GetAwaiter().OnCompleted(() =>
                {
                    QueryBatchStatusDto aQueryBatchStatusDto = null;
                    runningBatchListLockSlim.EnterWriteLock();
                    if (runningBatchList.ContainsKey(selectedQueryName))
                    {
                        if(runningBatchList.TryGetValue(selectedQueryName, out aQueryBatchStatusDto))
                            aQueryBatchStatusDto.cancellationTokenSource.Dispose();
                        runningBatchList.Remove(selectedQueryName);
                    }
                    runningBatchListLockSlim.ExitWriteLock();
                });
                


            }
            else // wenn kein Exportsettings, dann kann auch kein Batch ausgeführt werden -> Fehlermeldung
            {
                MessageBox.Show("Es sind keine Exportsettings definiert. Die Erstellung einer Datei ist nicht möglich", "Fehler bei Export");
                return;
            }
        }

        private bool canActionDelete(object arg)
        {
            if (queryBatchFileItemList.Count == 0) return false;
            string queryname = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            return !currentEditList.Contains(queryname);
        }

        private void onActionDelete(object obj)
        {
            if (queryBatchFileItemList.Count == 0) return;
            string selectedQueryName = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];

            //MessageBoxResult r = MessageBox.Show("Soll die Abfrage inkl. Datei wirklich gelöscht werden??", "Wirklich löschen", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            //if (r != MessageBoxResult.OK)
            //    return;


            int btn = -1;
            CustomMessageBoxParameter parameter = new CustomMessageBoxParameter
            {
                title = Resources.L073_DeleteAreYouSure,
                messageType = CustomMessageBoxParameter.MessageTypExclamationRed,
            };
            parameter.messageText = string.Format("{0}\n", Resources.M012_DeleteQuery);
            parameter.windowWidth = 450;
            parameter.windowHeight = 200;
            parameter.buttonList = new CustomMessageBoxButtonDescriptor[2];
            parameter.buttonList[0] = new CustomMessageBoxButtonDescriptor(Resources.L074_OK, true);
            parameter.buttonList[0].isDefaultKeystroke = true;
            parameter.buttonList[1] = new CustomMessageBoxButtonDescriptor(Resources.L040_Cancel, true);
            parameter.buttonList[1].isDefaultKeystroke = false;
            parameter.buttonList[1].isCancelKeystroke = true;
            parameter.buttonList[0].buttonActionCommand = (x) => { btn = 0; };
            parameter.buttonList[1].buttonActionCommand = (x) => { btn = 1; };
            _viewManager.showMesgBoxError(parameter);
            if (btn == 1)
            {
                return;
            }

            QueryBatchFileDto foundQueryBatchFileDto = null;
            foreach (QueryBatchFileDto item in _queryBatchFileDtoList)
            {
                if (item.queryName.Equals(selectedQueryName))
                {
                    foundQueryBatchFileDto = item;
                    break;
                }
            }
            dataService.deleteQueryFile(foundQueryBatchFileDto);
            readQueryfileData();
        }

        private bool canActionEdit(object arg)
        {
            if (queryBatchFileItemList.Count == 0) return false;
            string queryname = queryBatchFileItemList[selectedQueryBatchFileItemListIndex];
            return !currentEditList.Contains(queryname);
        }

        private void onActionEdit(object obj)
        {
            openTabItem();
        }

        private bool canActionAddNew(object arg)
        {
            return true;
        }

        /// <summary>
        /// Wenn ein neuer Tab erstellt werden soll
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void onActionAddNew(object obj)
        {
            addNewTabItem();
            selectedEditorTabIndex = editorTabList.Count - 1;//neuen selectieren
        }
        #endregion




    }




}
