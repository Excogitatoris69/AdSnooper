using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using CoreAdSnooper.common;
using CoreAdSnooper.interfaces;
using DomainAdSnooper.dto;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;

namespace AdSnooperGui.viewmodels
{
    public class ExportWindowVM : BasicVM
    {
        public bool isHeader { get; set; }
        public int selectedDelimiterIndex { get; set; }
        public int selectedOutputIndex { get; set; }
        public int selectedDestinationIndex { get; set; }
        public string filePath { get; set; }
        public List<DelimiterListItem> delimiterList { get; set; }
        public List<ExportOutputFormatListItem> outputList { get; set; }
        public List<ExportDestinationListItem> destinationList { get; set; }
        public ObservableCollection<CheckableListItem> propertyList { get; set; }


        //private ViewManager viewManager = null;
        //private string[] exportSettings;
        private ExportSettingsDto exportSettingsDto = null;
        private IDataService dataService;
        private ExportJobParameterDto exportJobParameter;
        //private ExportClipboardController exportClipboardController = null;
        private bool exportInProgress = false;
        //private SettingsController settingsController = null;
        private AppSettings appSettings = null;

        public ExportWindowVM(IDataService dataService, ExportJobParameterDto exportJobParameter, AppSettings appSettings)
        {
            this.dataService = dataService;
            this.appSettings = appSettings;
            this.exportJobParameter = exportJobParameter;
            //this.exportClipboardController = new ExportClipboardController();
            //this.viewManager = viewManager;
            init();
        }

        /// <summary>
        /// Hier kann das Schließen verhindert werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void voteAgainstWindowClosing(object sender, CancelEventArgs e)
        {
            //if(isUnsavedData)
            //    e.Cancel = true;
            //e.Cancel = false;
        }

        public void onCloseWindow()
        {
            saveSettings();
        }

        private void init()
        {
            if (appSettings.B001_Language == ELanguageSet.German.ToString())
            {
                var vCulture = new CultureInfo("de-DE");
                Resources.Culture = vCulture;
            }
            else
            {
                var vCulture = new CultureInfo("en-US");
                Resources.Culture = vCulture;
            }
            windowTitle = Resources.L039_Export + " - AD Snooper";
            isHeader = true;
            delimiterList = new List<DelimiterListItem>(4);
            delimiterList.Add(new DelimiterListItem(EDelimiter.Semicolon, Resources.L045_Semikolon));
            delimiterList.Add(new DelimiterListItem(EDelimiter.Comma, Resources.L046_Comma));
            delimiterList.Add(new DelimiterListItem(EDelimiter.Space, Resources.L047_Space));
            delimiterList.Add(new DelimiterListItem(EDelimiter.Tabulator, Resources.L048_Tabulator));
            outputList = new List<ExportOutputFormatListItem>(2);
            outputList.Add(new ExportOutputFormatListItem(EExportOutputFormat.CSV, "CSV"));
            outputList.Add(new ExportOutputFormatListItem(EExportOutputFormat.CSVTransformed, "CSV (Trans)"));
            destinationList = new List<ExportDestinationListItem>(2);
            destinationList.Add(new ExportDestinationListItem(EExportDestination.Clipboard, Resources.L050_Clipboard));
            destinationList.Add(new ExportDestinationListItem(EExportDestination.File, Resources.L049_File));
                        
            propertyList = new ObservableCollection<CheckableListItem>();
            
            foreach(string columnName in exportJobParameter.columnList)
            {
                propertyList.Add(new CheckableListItem(columnName, true));
            }
            //
            //set to gui
            if (exportJobParameter.withHeader)
                isHeader = true;
            else
                isHeader = false;
            //delimiter
            if (exportJobParameter.delimiter == EDelimiter.Semicolon) selectedDelimiterIndex = 0;
            if (exportJobParameter.delimiter == EDelimiter.Comma) selectedDelimiterIndex = 1;
            if (exportJobParameter.delimiter == EDelimiter.Space) selectedDelimiterIndex = 2;
            if (exportJobParameter.delimiter == EDelimiter.Tabulator) selectedDelimiterIndex = 3;
            //output
            if (exportJobParameter.outputFormat == EExportOutputFormat.CSV) selectedOutputIndex = 0;
            if (exportJobParameter.outputFormat == EExportOutputFormat.CSVTransformed) selectedOutputIndex = 1;
            //destination
            if (exportJobParameter.exportDestination == EExportDestination.Clipboard) selectedDestinationIndex = 0;
            if (exportJobParameter.exportDestination == EExportDestination.File) selectedDestinationIndex = 1;
            //filepath
            filePath = exportJobParameter.filePath;
            //properties
            int propIndex = 0;
            foreach (CheckableListItem propertyListItem in propertyList)
            {
                propertyListItem.isChecked = exportJobParameter.columnSelectedList[propIndex++];
            }

        }


        /// <summary>
        /// Save Gui to Settings
        /// </summary>
        public void saveSettings()
        {
            exportJobParameter.withHeader = isHeader;
            exportJobParameter.delimiter = delimiterList[selectedDelimiterIndex].delimiterType;
            exportJobParameter.outputFormat = outputList[selectedOutputIndex].formatType;
            exportJobParameter.exportDestination = destinationList[selectedDestinationIndex].destinationType;
            exportJobParameter.filePath = filePath;
            int propIndex = 0;
            foreach (CheckableListItem propertyListItem in propertyList)
            {
                exportJobParameter.columnSelectedList[propIndex++] = propertyListItem.isChecked;
            }
            if(exportJobParameter.dataType== ExportJobParameterDto.DATATYPE_PERSON)
                appSettings.setExportParameterPerson(exportJobParameter);
            else if (exportJobParameter.dataType == ExportJobParameterDto.DATATYPE_GROUP)
                appSettings.setExportParameterGroup(exportJobParameter);
            else
                appSettings.setExportParameter(exportJobParameter);
            appSettings.writeSettings();
        }

        public void onButtonSelectAllProperties()
        {
            foreach(CheckableListItem item in propertyList)
            {
                item.isChecked = true;
            }
            
        }
        public void onButtonSelectNoneProperties()
        {
            foreach (CheckableListItem item in propertyList)
            {
                item.isChecked = false;
            }
        }
        public void onButtonInvertSelectionProperties()
        {
            foreach (CheckableListItem item in propertyList)
            {
                item.isChecked = !item.isChecked;
            }
        }

        private char getDelimiterChar()
        {
            EDelimiter item = delimiterList[selectedDelimiterIndex].delimiterType;
            if (item == EDelimiter.Space)
                return ' ';
            if (item == EDelimiter.Semicolon)
                return ';';
            if (item == EDelimiter.Comma)
                return ',';
            else
                return '\t';
        }

        private string generateExportDataAsTransformedCsvString()
        {
            StringBuilder rowData = new StringBuilder();
            StringBuilder resultData = new StringBuilder();
            char delimiter = getDelimiterChar();
            //ob eine Zelle ausgegeben werden soll, hängt davon ab, ob sie angehakt ist.
            bool[] selectedColumns = new bool[propertyList.Count];
            int index = 0;
            foreach (CheckableListItem item in propertyList)
            {
                selectedColumns[index++] = item.isChecked;
            }
            index = 0;
            for (int col = 0; col < exportJobParameter.dataTable.GetLength(1); col++)
            {
                rowData.Clear();
                bool first = true;
                if (selectedColumns[col]) //nur, wenn angehakt
                {
                    //header
                    if (isHeader)
                    {
                        rowData.Append(exportJobParameter.columnList[col]);
                        rowData.Append(delimiter);
                    }

                    for (int row = 0; row < exportJobParameter.dataTable.GetLength(0); row++)
                    {
                        if (first) first = false;
                        else rowData.Append(delimiter);
                        rowData.Append(exportJobParameter.dataTable[row, col]);
                    }
                }
                resultData.Append(rowData);
                resultData.Append(Environment.NewLine);
            
            }
            return resultData.ToString();
        }


        private string generateExportDataAsCsvString()
        {
            StringBuilder rowData = new StringBuilder();
            StringBuilder resultData = new StringBuilder();
            char delimiter = getDelimiterChar();
            //ob eine Zelle ausgegeben werden soll, hängt davon ab, ob sie angehakt ist.
            bool[] selectedColumns = new bool[propertyList.Count];
            int index = 0;
            foreach (CheckableListItem item in propertyList)
            {
                selectedColumns[index++] = item.isChecked;
            }
            index = 0;
            if (isHeader)
            {
                bool first = true;
                foreach(bool item in selectedColumns)
                {
                    if (item)//nur, wenn angehakt
                    {
                        if (first) first = false; 
                        else resultData.Append(delimiter);
                        resultData.Append(exportJobParameter.columnList[index]);
                    }
                    index++;
                }
                resultData.Append(Environment.NewLine);
            }
            //data
            for (int row = 0; row < exportJobParameter.dataTable.GetLength(0); row++)
            {
                rowData.Clear();
                bool first = true;
                for (int col = 0; col < exportJobParameter.dataTable.GetLength(1); col++)
                {
                    if (selectedColumns[col]) //nur, wenn angehakt
                    { 
                        if (first) first = false;
                        else rowData.Append(delimiter);
                        rowData.Append(exportJobParameter.dataTable[row, col]);
                    }
                }
                resultData.Append(rowData);
                resultData.Append(Environment.NewLine);
            }
            return resultData.ToString();
        }

        public void onButtonExport()
        {
            if (exportInProgress) return;
            Cursor currentCursor = Mouse.OverrideCursor;
            bool export = true;
            if (exportJobParameter.exportDestination == EExportDestination.File)
                export=showFileSelectDialog();
            if (export)
            {
                string outputData = null;
                exportInProgress = true;
                Mouse.OverrideCursor = Cursors.Wait;
                if(outputList[selectedOutputIndex].formatType == EExportOutputFormat.CSV)
                    outputData = generateExportDataAsCsvString();
                else
                    outputData = generateExportDataAsTransformedCsvString();
                Mouse.OverrideCursor = currentCursor;
                if (exportJobParameter.exportDestination == EExportDestination.Clipboard)
                    Clipboard.SetText(outputData);
                else
                    saveToFile(outputData);
            }
            exportInProgress = false;
        }   

        private void saveToFile(string data)
        {
            //todo savetofile verbessern mit try catch und messagebox
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(data);
                }
            }
            catch (Exception)
            {
               //
            }
        }


        public void onButtonClose()
        {
            closeWindow();
        }

        private bool showFileSelectDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv|Text file (*.txt)|*.txt|*.* (*.*)|*.*";
            string currentDir = null;
            //currentDir = Path.GetDirectoryName(exportWindowParameter.filePath);
            currentDir = exportJobParameter.filePath;
            //saveFileDialog.FileName = Path.GetFileName(exportSettingsDto.filePathPerson);
            //currentDir = Path.GetDirectoryName(exportSettingsDto.filePathGroup);
            //saveFileDialog.FileName = Path.GetFileName(exportSettingsDto.filePathGroup);
            //saveFileDialog.DefaultDirectory = currentDir;
            saveFileDialog.InitialDirectory = currentDir;
            saveFileDialog.Title = Resources.L039_Export;
            if (saveFileDialog.ShowDialog() == true)
            {
                exportJobParameter.filePath = Path.GetDirectoryName(saveFileDialog.FileName);
                appSettings.B005_ExportFilePersons = Path.GetDirectoryName(saveFileDialog.FileName);
                filePath = saveFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }
        /*
        private void exportToClipboard()
        {
            if (exportWindowParameter.dataType == ExportWindowParameter.DATATYPE_PERSON)
            {
                ExportResultDto result =  exportClipboardController.exportPersonList(exportSettingsDto, dataService.currentFoundPersonData);
            }
            else
            {
                ExportResultDto result = exportClipboardController.exportGroupList(exportSettingsDto, dataService.currentFoundGroupData);
            }


        }
        */
    }

}
