using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;

namespace AdSnooperGui.viewmodels
{
    public class GroupDiffWindowVM: BasicVM
    {
        public List<DynamicDataTableColumn> columnList { get; set; }
        public ObservableCollection<DynamicDataTableRow> rowList { get; set; }

        private GroupDiffWindowParameter _parameter = null;
        public GenericCommand cmdGroupCopyClipboard { get; set; }
        public GenericCommand cmdGroupCopyNameClipboard { get; set; }

        public List<DynamicDataTableRow> currentSelectedRowList = null;
        private AppSettings appSettings = null;

        public GroupDiffWindowVM(GroupDiffWindowParameter groupDiffWindowParameter, AppSettings appSettings)
        {
            _parameter = groupDiffWindowParameter;
            this.appSettings = appSettings;
            init();
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
            windowTitle = Resources.L063_DiffGroups + " - AD Snooper";
            cmdGroupCopyClipboard = new GenericCommand(onActionGroupCopyClipboard, canActionGroupCopyClipboard);
            cmdGroupCopyNameClipboard = new GenericCommand(onActionGroupCopyNameClipboard, canActionGroupCopyNameClipboard);
            currentSelectedRowList = new List<DynamicDataTableRow>();
            initData();
        }

        private void onActionGroupCopyClipboard(object param)
        {
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;
            foreach (string item in _parameter.columnHeader)
            {
                if (firstItem)
                    firstItem = false;
                else
                    sb.Append('\t');
                sb.Append(item);
            }
            sb.Append(Environment.NewLine);
            /*
            for (int r=0; r < _parameter.data.GetLength(0); r++)
            {
                firstItem = true;
                for (int c = 0; c < _parameter.data.GetLength(1); c++)
                {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append('\t');
                    sb.Append(_parameter.data[r,c]);
                }
                sb.Append(Environment.NewLine);
            }
            */
            //currentSelectedRowList
            foreach (DynamicDataTableRow row in currentSelectedRowList)
            {
                firstItem = true;
                for (int c = 1; c < row.cellData.Length; c++)
                {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append('\t');
                    sb.Append(row.cellData[c]);
                }
                sb.Append(Environment.NewLine);
            }

            Clipboard.SetText(sb.ToString());
        }

        internal bool canActionGroupCopyClipboard(object parameter)
        {
            return true;
        }

        private void onActionGroupCopyNameClipboard(object param)
        {
            StringBuilder sb = new StringBuilder();
            bool firstItem = true;
            foreach(DynamicDataTableRow row in currentSelectedRowList)
            {
                if (firstItem)
                    firstItem = false;
                else
                    sb.Append(',');
                sb.Append(row.cellData[1]);
            }

            Clipboard.SetText(sb.ToString());
        }

        internal bool canActionGroupCopyNameClipboard(object parameter)
        {
            return true;
        }

        private void initData()
        {
            columnList = new List<DynamicDataTableColumn>(_parameter.columnHeader.Length + 1);
            columnList.Add(new DynamicDataTableColumn("#", string.Format("cellData[{0}]", 0), 50));//#
            for (int c = 0; c < _parameter.columnHeader.Length; c++)
            {
                columnList.Add(new DynamicDataTableColumn(_parameter.columnHeader[c], string.Format("cellData[{0}]", c+1), 250));
            }
            rowList = new ObservableCollection<DynamicDataTableRow>();
            int colSize = _parameter.columnHeader.Length + 1;
            int rowSize = _parameter.data.GetLength(0);
            int numberOfDigits = (int)Math.Floor(Math.Log10(rowSize) + 1);
            int lineNumber = 0;
            for (int r = 0; r < rowSize; r++)
            {
                lineNumber = r + 1;
                DynamicDataTableRow row = new DynamicDataTableRow(colSize);

                row.cellData[0] = lineNumber.ToString("D" + numberOfDigits);//#
                for (int c = 0; c < _parameter.columnHeader.Length; c++)
                {
                    row.cellData[c+1] = _parameter.data[r,c];
                }
                rowList.Add(row);
            }
        }
        


    }
    
    

}
