using ICSharpCode.AvalonEdit.Highlighting;

namespace AdSnooperGui.models
{

    public class AdSnooperHighlightingDefinition : IHighlightingDefinition
    {
        public string Name { get; set; }

        public HighlightingRuleSet MainRuleSet { get; set; }

        public IEnumerable<HighlightingColor> NamedHighlightingColors { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public HighlightingColor GetNamedColor(string name)
        {
            HighlightingColor result = null;
            foreach (HighlightingColor item in NamedHighlightingColors)
            {
                if (item.Name.Equals(name))
                {
                    result = item;
                    break;
                }
            }
            //throw new NotImplementedException();
            return result;
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            //MainRuleSet.
            throw new NotImplementedException();
        }
    }

}
