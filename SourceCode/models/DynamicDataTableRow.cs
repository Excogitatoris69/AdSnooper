using System.Text;

namespace AdSnooperGui.models
{

    /// <summary>
    /// Speichert die Daten einer Tabelle als dynamisches Array.
    /// </summary>
    public class DynamicDataTableRow
    {
        private StringBuilder _fullRowData = null;
        public DynamicDataTableRow(int numberOfColumns)
        {
            _fullRowData = new StringBuilder();
            _cellData = new string[numberOfColumns];//alle Zellen einer Row
        }

        string[] _cellData;
        public string[] cellData {
            get {  return _cellData; }
            set { 
                _cellData = value;
                buildFullRowContent();
            }
        }

        public int rowNumber { get; set; }

        /// <summary>
        /// Erstellt aus allen Daten eine Zeile, um eine Volltextsuche zu realisieren.
        /// </summary>
        private void buildFullRowContent()
        {
            if (_cellData != null && _cellData.Length > 0)
            {
                _fullRowData.Clear();
                foreach (string cellValue in _cellData)
                {
                    _fullRowData.Append(cellValue);
                    _fullRowData.Append(' ');
                }
            }
        }

        public string getFullRowContent()
        {
            if (_fullRowData.Length == 0)
                buildFullRowContent();
            return _fullRowData.ToString();
        }
    }

}
