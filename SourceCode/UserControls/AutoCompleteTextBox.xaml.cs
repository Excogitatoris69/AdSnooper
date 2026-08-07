using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace AdSnooperGui.UserControls
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextBox.xaml
    /// </summary>
    public partial class AutoCompleteTextBox : UserControl
    {

        public AutoCompleteTextBox()
        {
            InitializeComponent();
            //DataContext: niemals setzen. Kommt vom Parent!
        }

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(nameof(AutoCompleteTextBox.Text), typeof(string),  
                typeof(AutoCompleteTextBox), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        public string Text
        {
            get => (string)GetValue(TextProperty); 
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty AutoSuggestionListProperty =
            DependencyProperty.Register(nameof(AutoCompleteTextBox.AutoSuggestionList), typeof(List<string>),
                typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(null)
                );
        public List<string> AutoSuggestionList
        {
            get => (List<string>)GetValue(AutoSuggestionListProperty);
            set => SetValue(AutoSuggestionListProperty, value);
        }

        public static readonly DependencyProperty SuggestionListHeightProperty =
            DependencyProperty.Register(nameof(AutoCompleteTextBox.SuggestionListHeight), typeof(string),
                typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(string.Empty)
                );
        public string SuggestionListHeight
        {
            get => (string)GetValue(SuggestionListHeightProperty);
            set => SetValue(SuggestionListHeightProperty, value);
        }

        public static readonly DependencyProperty SuggestionListWidthProperty =
            DependencyProperty.Register(nameof(AutoCompleteTextBox.SuggestionListWidth), typeof(string),
                typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(string.Empty)
                );
        public string SuggestionListWidth
        {
            get => (string)GetValue(SuggestionListWidthProperty);
            set => SetValue(SuggestionListWidthProperty, value);
        }

        public static readonly DependencyProperty SuggestionListMaxEntriesProperty =
            DependencyProperty.Register(nameof(AutoCompleteTextBox.SuggestionListMaxEntries), typeof(int),
                typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(10)
                );
        public int SuggestionListMaxEntries
        {
            get => (int)GetValue(SuggestionListMaxEntriesProperty);
            set => SetValue(SuggestionListMaxEntriesProperty, value);
        }


        /// <summary>  
        ///  Open Auto Suggestion box method  
        /// </summary>  
        private void openAutoSuggestionBox()
        {
            try
            {
                this.autoListPopup.Visibility = Visibility.Visible;
                this.autoListPopup.IsOpen = true;
                this.autoList.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>  
        ///  Close Auto Suggestion box method  
        /// </summary>  
        private void closeAutoSuggestionBox()
        {
            try
            {
                this.autoListPopup.Visibility = Visibility.Collapsed;
                this.autoListPopup.IsOpen = false;
                this.autoList.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
            }
        }

        

        /// <summary>  
        ///  Auto Text Box text changed method.  
        /// </summary>  
        /// <param name="sender">Sender parameter</param>  
        /// <param name="e">Event parameter</param>  
        private void AutoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int dum = 0;
            try
            {
                // Verification.  
                if (string.IsNullOrEmpty(autoTextBox.Text))
                {
                    // Disable.  
                    closeAutoSuggestionBox();
                    return;
                }
                if (AutoSuggestionList.Count > 0)
                {
                    // Enable.  
                    openAutoSuggestionBox();

                    // Settings.  
                    //autoList.ItemsSource = AutoSuggestionList.Where(p => p.ToLower().Contains(autoTextBox.Text.ToLower())).ToList();
                    autoList.ItemsSource = AutoSuggestionList.Where(p => p.Contains(autoTextBox.Text, StringComparison.OrdinalIgnoreCase)).ToList().Take(SuggestionListMaxEntries);//50
                    moveListItemSelection(0);
                }
                else
                {
                    dum = 0;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Up und Down durch Liste sowie Enter für Auswahl übernehmen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoTextBox_KeyPressed(object sender, KeyEventArgs e)
        {
            try
            {
                Key myKey = e.Key;
                if (myKey == Key.Up)
                {
                    int newIndex = autoList.SelectedIndex - 1;
                    moveListItemSelection(newIndex); 
                    
                }
                if (myKey == Key.Down)
                {
                    int newIndex = autoList.SelectedIndex + 1;
                    moveListItemSelection(newIndex);
                    
                }
                if (myKey == Key.Enter)
                {

                    // Settings.
                    object selectedItem = autoList.SelectedItem;
                    if (selectedItem != null)
                    {
                        this.autoTextBox.Text = selectedItem.ToString();
                    }
                    //this.autoList.SelectedIndex = -1;
                    // Disable.  
                    this.closeAutoSuggestionBox();
                }
                if (myKey == Key.Escape)
                {
                    closeAutoSuggestionBox();
                }
                //if (myKey == Key.Home)
                //{
                //    int newIndex = 0;
                //    moveListItemSelection(newIndex);
                //}
                //if (myKey == Key.End)
                //{
                //    int newIndex = autoList.Items.Count - 1;
                //    moveListItemSelection(newIndex);
                //}
                //if (myKey == Key.PageUp)
                //{
                //    int newIndex = autoList.SelectedIndex - 5;
                //    moveListItemSelection(newIndex);
                //}
                //if (myKey == Key.PageDown)
                //{
                //    int newIndex = autoList.SelectedIndex + 5;
                //    moveListItemSelection(newIndex);
                //}
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Bei Auswahl mir Maus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoList_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verification.  
                if (this.autoList.SelectedIndex <= -1) 
                {
                    // Disable.  
                    this.closeAutoSuggestionBox();
                    return;
                }
                // Settings.  
                this.autoTextBox.Text = this.autoList.SelectedItem.ToString();
                // Disable.  
                this.closeAutoSuggestionBox();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Bei Enter anstatt Mausklick inerhalb der Liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Key myKey = e.Key;
                if (myKey == Key.Enter)
                {
                    // Settings.
                    object selectedItem = autoList.SelectedItem;
                    if (selectedItem != null)
                    {
                        this.autoTextBox.Text = selectedItem.ToString();
                    }
                    // Disable.  
                    this.closeAutoSuggestionBox();
                }
                if (myKey == Key.Escape)
                {
                    closeAutoSuggestionBox();
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Bewegt die LietSelection noch oben oder unten.
        /// </summary>
        /// <param name="newIndex"></param>
        private void moveListItemSelection(int newIndex)
        {
            try
            {
                if (newIndex  < 0)
                {
                    newIndex = 0;
                }
                if (newIndex >= autoList.Items.Count)
                {
                    newIndex = autoList.Items.Count - 1;
                }
                if (autoList.Visibility != Visibility.Visible) return;
                if (!autoListPopup.IsOpen) return;
                autoList.SelectedItem = autoList.Items[newIndex];
                autoList.UpdateLayout();


                //((ListBoxItem)autoList.ItemContainerGenerator.ContainerFromIndex(newIndex)).Focus();
                ListBoxItem t1 = ((ListBoxItem)autoList.ItemContainerGenerator.ContainerFromIndex(newIndex));
                if(t1!=null)
                    t1.Focus();
                
                
                autoTextBox.Focus();
            }
            catch (Exception)
            {
                
            }
            
        }


    }
}
