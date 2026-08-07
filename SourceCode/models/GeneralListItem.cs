namespace AdSnooperGui.models
{
    public class GeneralListItem
    {
        public string displayName { get; set; }
        public string value { get; set; }
        public GeneralListItem(string displayname, string value)
        {
            this.displayName = displayname;
            this.value = value;
        }
    }

}
