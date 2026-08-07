using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace AdSnooperGui.models
{
    public class AsqlCompletionData : ICompletionData
    {
        public AsqlCompletionData(string text)
        {
            this.Text = text;
        }

        public AsqlCompletionData(string text, string description)
        {
            this.Text = text;
            this.Description = description;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the drop down list.
        public object Content
        {
            get { return this.Text; }
        }

        //public object Description
        //{
        //    get { return "Description for " + this.Text; }
        //}

        public double Priority { get { return 0; } }

        public object Description { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
            
        }

        public override bool Equals(object? obj)
        {
            return obj is AsqlCompletionData data &&
                   Text == data.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }

    }

    /// <summary>
    /// Sortiert alphabetisch aufwärts
    /// </summary>
    public class AsqlCompletionDataComparer : IComparer<AsqlCompletionData>
    {
        public int Compare(AsqlCompletionData? x, AsqlCompletionData? y)
        {
            return x.Text.CompareTo(y.Text);
        }
    }
}
