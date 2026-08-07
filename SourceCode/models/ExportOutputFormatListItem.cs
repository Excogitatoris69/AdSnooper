using AdSnooperGui.common;
using DomainAdSnooper.dto;

namespace AdSnooperGui.models
{
    public class ExportOutputFormatListItem
    {
        public EExportOutputFormat formatType { get; set; }
        public string displayName { get; set; }
        public ExportOutputFormatListItem(EExportOutputFormat formatType, string displayName)
        {
            this.formatType = formatType;
            this.displayName = displayName;
        }
    }

}
