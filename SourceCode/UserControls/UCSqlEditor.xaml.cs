using AdSnooperGui.models;
using AdSnooperGui.viewmodels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace AdSnooperGui.UserControls
{
    /// <summary>
    /// Interaction logic for UCSqlEditor.xaml
    /// </summary>
    public partial class UCSqlEditor : UserControl
    {

        public SqlEditorVM viewmodel { get; set; }
        private CompletionWindow completionWindow = null;

        public UCSqlEditor()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
        }
        public UCSqlEditor(SqlEditorVM viewmodel)
        {
            InitializeComponent();
            this.viewmodel = viewmodel;
            DataContext = this.viewmodel;
            init();

        }

        private void init()
        {
            
            viewmodel.OnTableDataChangedEvent += OnTableDataChanged;
            //avalon editor
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            //textEditor.TextArea.TextInput += TextArea_TextCopied;
            //textEditor.TextArea.change
            viewmodel.PropertyChanged += onViewModelPropertyChanged;
            textEditor.SyntaxHighlighting = viewmodel.highlightingDefinition;
            viewmodel.isUnsafedChanges = true;

            DataObject.AddPastingHandler(this, PasteEvent);

        }

        private void OnTableDataChanged(object? sender, TableDataChangedEventArgs e)
        {
            DataGridTextColumn aDataGridTextColumn = null;
            try
            {
                dataGridResultTable.Columns.Clear();
                foreach (DynamicDataTableColumn tabColumn in viewmodel.columnList)
                {
                    aDataGridTextColumn = new DataGridTextColumn { Header = tabColumn.header, Binding = new Binding(tabColumn.binding), Width = new DataGridLength(tabColumn.width) };
                    dataGridResultTable.Columns.Add(aDataGridTextColumn);
                }
                if (e.rowIndex >= 0)
                    dataGridResultTable.SelectedIndex = e.rowIndex;
                dataGridResultTable.Focus();
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void PasteEvent(object sender, DataObjectPastingEventArgs e)
        {
            viewmodel.isUnsafedChanges = true;
        }

        private void onViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(viewmodel.avalonEditUploadText)))
            {
                textEditor.Text = viewmodel.avalonEditUploadText;
            }
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            viewmodel.isUnsafedChanges = true;
            int myKey = (int)(ModifierKeys.Control | ModifierKeys.Shift);
            int curKeyMod = (int)Keyboard.Modifiers;
            if (e.Text == " " && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int offset = textEditor.CaretOffset;
                //int wordBegin = getWordBeginPosition(textEditor.Text, offset);
                textEditor.CaretOffset = offset-1;
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.MaxHeight = 200;
                completionWindow.Height = 200;
                completionWindow.Width = 200;
                completionWindow.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#ccccff"));
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                List<AsqlCompletionData> myData = viewmodel.sqlAdFieldCompletionData; 
                foreach (AsqlCompletionData item in myData)
                {
                    data.Add(item);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
                //textEditor.Document.Remove(wordBegin,(offset-wordBegin));
            }
            if (e.Text == " " && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                int offset = textEditor.CaretOffset;
                //int wordBegin = getWordBeginPosition(textEditor.Text, offset);
                textEditor.CaretOffset = offset - 1;
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.MaxHeight = 200;
                completionWindow.Height = 200;
                completionWindow.Width = 200;
                completionWindow.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#ccccff"));
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                List<AsqlCompletionData> myData = viewmodel.sqlCommandCompletionData;
                foreach (AsqlCompletionData item in myData)
                {
                    data.Add(item);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };

            }

            if (e.Text == "'")
            {
                int offset = textEditor.CaretOffset;
                textEditor.Document.Insert(textEditor.CaretOffset, "'");
                offset = textEditor.CaretOffset;
                textEditor.CaretOffset = offset - 1;
            }


        }


        private int getWordBeginPosition(string text, int pos)
        {
            int posOfSpace = 0;
            for(int x = pos - 2; x > 0; x--)
            {
                if(text.Substring(x,1).Equals(" "))
                {
                    posOfSpace = x;
                    break;
                }
            }
            posOfSpace++;
            return posOfSpace;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.editorText = textEditor.Text;
            textEditor.Focus();
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.editorText = textEditor.Text;
        }

        private void ButtonCloseTab_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.editorText = textEditor.Text;
            textEditor.Focus();
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            viewmodel.isUnsafedChanges = true;
        }

        private void window_handleKeystrokeEvents(object sender, KeyEventArgs e)
        {
            viewmodel.editorText = textEditor.Text;
            viewmodel.handleKeystrokeEvents(sender, e);
        }

        /// <summary>
        /// Wenn an den Spaltenbreiten etwas verändert wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridResultTable_LayoutUpdated(object sender, EventArgs e)
        {
            for(int colIndex=0; colIndex< dataGridResultTable.Columns.Count; colIndex++)
            {
                if (dataGridResultTable.Columns[colIndex].ActualWidth != viewmodel.columnSizes[colIndex])
                {
                    viewmodel.columnSizes[colIndex] = dataGridResultTable.Columns[colIndex].ActualWidth;
                }
            }
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //    _viewmodel.isUnsafedChanges = false;
        //}

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);
        //}

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //}

        //private void tbEditor_PreviewKeyUp(object sender, KeyEventArgs e)
        //{
        //    viewmodel.isUnsafedChanges = true;

        //}
    }
}
