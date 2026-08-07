using AdSnooperGui.common;
using DomainAdSnooper.dto;

namespace AdSnooperGui.models
{
    public class ExportDestinationListItem
    {
        public EExportDestination destinationType { get; set; }
        public string displayName { get; set; }
        public ExportDestinationListItem(EExportDestination destinationType, string displayName)
        {
            this.destinationType = destinationType;
            this.displayName = displayName;
        }
    }

}
