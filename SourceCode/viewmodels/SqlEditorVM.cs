using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using CoreAdSnooper.common;
using CoreAdSnooper.interfaces;
using DomainAdSnooper.dto;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Windows.Themes;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Color = System.Windows.Media.Color;

namespace AdSnooperGui.viewmodels
{
    public class SqlEditorVM : SqlQueryBasicVM, IDisposable
    {

        private int pageSizeFileExport = 10000;
        private int taskDelayWaittime = 10;
        private ViewManager _viewManager = null;
        int[] adAttributeIdListUser;
        int[] adAttributeIdListGroup;
        ExportJobParameterDto exportWindowParameter = null;


        public double [] columnSizes { get; set; }
        public ObservableCollection<DynamicDataTableColumn> columnList { get; set; }
        public ObservableCollection<DynamicDataTableRow> rowList { get; set; }
        
        private CancellationTokenSource runQueryCancellationTokenSource = null;

        private Visibility _runningQueryProgressbarVisibility;
        public Visibility runningQueryProgressbarVisibility
        {
            get
            {
                return _runningQueryProgressbarVisibility;

            }
            set
            {
                _runningQueryProgressbarVisibility = value;
                OnPropertyChanged(nameof(runningQueryProgressbarVisibility));
            }
        }




        private bool isQueryRunning = false;

        public string  editorText { get; set; }
        private bool _isUnsafedChanges;
        public bool isUnsafedChanges 
        { 
            get { return _isUnsafedChanges; }
            set
            {
                if(value != _isUnsafedChanges)
                {
                    _isUnsafedChanges = value;
                    if (value == true)
                        _viewManager.appSettings.unsafedDataInc();
                    else
                        _viewManager.appSettings.unsafedDataDec();

                }

            }
        }


        public string _queryTabTitle { get; set; }
        public string  queryTabTitle
        {
            get {
                if (string.IsNullOrEmpty(queryName))
                    return _queryTabTitle;
                else return queryName;
            }
            set
            {
                _queryTabTitle = value;
            }
        }

        public string _avalonEditUploadText { get; set; }
        public string avalonEditUploadText
        {
            get
            {
                return _avalonEditUploadText;
                
            }
            set
            {
                _avalonEditUploadText = value;
                OnPropertyChanged(nameof(avalonEditUploadText));
            }
        }

        //SqlParserResultDto
        private bool _deepSearch;
        public bool deepSearch
        {
            get
            {
                return _deepSearch;
            }
            set
            {
                if(value != _deepSearch)
                {
                    _deepSearch = value;
                    OnPropertyChanged(nameof(deepSearch));
                }
            }
        }

        //SqlParserResultDto
        private bool _sqlParsedSyntaxValid;
        public bool sqlParsedSyntaxValid
        {
            get
            {
                return _sqlParsedSyntaxValid;
            }
            set
            {
                _sqlParsedSyntaxValid = value;
                OnPropertyChanged(nameof(sqlParsedSyntaxValid));
            }
        }

        private string _sqlParsedAdQueryString;
        public string sqlParsedAdQueryString
        {
            get
            {
                return _sqlParsedAdQueryString;
            }
            set
            {
                _sqlParsedAdQueryString = value;
                OnPropertyChanged(nameof(sqlParsedAdQueryString));
            }
        }

        private string _sqlParseSyntaxErrorText;
        public string sqlParseSyntaxErrorText
        {
            get
            {
                return _sqlParseSyntaxErrorText;
            }
            set
            {
                _sqlParseSyntaxErrorText = value;
                OnPropertyChanged(nameof(sqlParseSyntaxErrorText));
            }
        }

        private int _pageSize;
        public int pageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                OnPropertyChanged(nameof(pageSize));
            }
        }

        public string  queryName{ get; set; }

        AdSnooperHighlightingDefinition _highlightingDefinition = null;
        public AdSnooperHighlightingDefinition highlightingDefinition {
            get
            {
                return _highlightingDefinition;
            }
        }


        public event EventHandler<SqlEditorButtonEventArgs> buttonEvent;

        public GenericCommand cmdRun { get; set; }
        public GenericCommand cmdCancel { get; set; }
        public GenericCommand cmdSave { get; set; }
        public GenericCommand cmdSaveAs { get; set; }
        public GenericCommand cmdCloseTab { get; set; }
        public GenericCommand cmdCopyClipboard { get; set; }
        public GenericCommand cmdOpenExportWindow { get; set; }


        public SqlEditorVM(IDataService dataService, ViewManager viewManager)
        {
            this.dataService = dataService;
            this._viewManager = viewManager;
            init();
        }

        private void init()
        {
            sqlParsedAdQueryString = "";
            sqlParsedSyntaxValid = false;
            _sqlParseSyntaxErrorText = "";
            runningQueryProgressbarVisibility = Visibility.Hidden;
            columnList = new ObservableCollection<DynamicDataTableColumn>();
            rowList = new ObservableCollection<DynamicDataTableRow>();
            pageSize = 2000;
            columnSizes = new double[60];
            for (int i = 0; i < 30; i++)
            {
                columnSizes[i] = 150;
            }
            columnSizes[0] = 50;

            //Commands
            cmdRun = new GenericCommand(onActionRun, canActionRun);
            cmdCancel = new GenericCommand(onActionCancel, canActionCancel);
            cmdSave = new GenericCommand(onActionSave, canActionSave);
            cmdSaveAs = new GenericCommand(onActionSaveAs, canActionSaveAs);
            cmdCloseTab = new GenericCommand(onActionCloseTab, canActionCloseTab);
            cmdCopyClipboard = new GenericCommand(onActionCopyClipboard, canActionCopyClipboard);
            cmdOpenExportWindow = new GenericCommand(onActionOpenExportWindow, canActionOpenExportWindow);
            //avaloneditor
            initHightlighting();
            buildSqlAdFieldCompletionData();
            buildSqlCommandCompletionData();

            adAttributeIdListUser = new int[dataService.adAttributeListUser.Count];
            adAttributeIdListGroup = new int[dataService.adAttributeListGroup.Count];
            int index = 0;
            foreach (AdAttributeDto item in dataService.adAttributeListUser)
            {
                adAttributeIdListUser[index++] = item.id;
            }
            index = 0;
            foreach (AdAttributeDto item in dataService.adAttributeListGroup)
            {
                adAttributeIdListGroup[index++] = item.id;
            }
        }

        /// <summary>
        /// Reagiert aus Tastaturereignisse, wenn eine Taste gedrückt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void handleKeystrokeEvents(object sender, KeyEventArgs e)
        {
            int shiftAndControlModifier = (int)(ModifierKeys.Control | ModifierKeys.Shift);
            int controlModifier = (int)ModifierKeys.Control;
            int shiftModifier = (int)ModifierKeys.Shift;
            int myModifiers = (int)Keyboard.Modifiers;
            if (e.Key == Key.F5 && ((myModifiers & shiftModifier) == shiftModifier))
            {
                onActionCancel();
            }
            else if (e.Key == Key.F5)
            {
                onActionRun();
            }
            //if (e.Key == Key.S && ((myModifiers & shiftAndControlModifier) == shiftAndControlModifier))
            //{
            //    onActionSaveAs();
            //}
            if (e.Key == Key.S && ((myModifiers & controlModifier) == controlModifier))
            {
                onActionSave();
            }
            if (e.Key == Key.W && ((myModifiers & controlModifier) == controlModifier))
            {
                onActionCloseTab();
            }
            
        }

        protected void raiseButtonEvent(SqlEditorButtonEventArgs eventArgs)
        {
            buttonEvent?.Invoke(this, eventArgs);
        }

        public void Dispose()
        {
            
        }

    }


    

}
