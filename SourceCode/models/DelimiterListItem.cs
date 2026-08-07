using AdSnooperGui.common;
using DomainAdSnooper.dto;

namespace AdSnooperGui.models
{
    public class DelimiterListItem
    {
        public EDelimiter delimiterType { get; set; }
        public string displayName { get; set; }
        public DelimiterListItem(EDelimiter delimiterType, string displayName)
        {
            this.delimiterType = delimiterType;
            this.displayName = displayName;
        }
    }

}
