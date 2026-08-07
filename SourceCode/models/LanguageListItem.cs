using AdSnooperGui.common;

namespace AdSnooperGui.models
{
    public class LanguageListItem
    {
        public string displayName { get; set; } //Deutsch, Englisch
        public ELanguageSet languageSet { get; set; } 

        public LanguageListItem(string displayName, ELanguageSet languageSet)
        {
            this.displayName = displayName;
            this.languageSet = languageSet;
        }
    }

}
