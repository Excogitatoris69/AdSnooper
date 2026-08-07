using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdSnooperGui.models
{
    public class TableDataChangedEventArgs
    {
        public int tableId { get; set; }
        public int rowIndex { get; set; }

        public static readonly int TABLE_ID_PERSONSEARCHRESULT = 1;
        public static readonly int TABLE_ID_GROUPSEARCHRESULT = 2;
    }
}
