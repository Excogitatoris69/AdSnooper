using AdSnooperGui.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AdSnooperGui.models
{
    public class SqlEditorTabItem : TabItem
    {
        public event EventHandler closeTabEvent;

        private UCCloseableTabItemHeader closeableTabHeader = null;

        public SqlEditorTabItem()
        {
            closeableTabHeader = new UCCloseableTabItemHeader();
            this.Header = closeableTabHeader;

            // Attach to the CloseableHeader events
            // (Mouse Enter/Leave, Button Click, and Label resize)
            closeableTabHeader.button_close.MouseEnter +=
               new MouseEventHandler(button_close_MouseEnter);
            closeableTabHeader.button_close.MouseLeave +=
               new MouseEventHandler(button_close_MouseLeave);
            closeableTabHeader.button_close.Click +=
               new RoutedEventHandler(button_close_Click);
            closeableTabHeader.label_TabTitle.SizeChanged +=
               new SizeChangedEventHandler(label_TabTitle_SizeChanged);
        }


        public string HeaderText
        {
            set
            {
                ((UCCloseableTabItemHeader)this.Header).label_TabTitle.Content = value;
            }
        }

        // Override OnSelected - Show the Close Button
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((UCCloseableTabItemHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }

        // Override OnUnSelected - Hide the Close Button
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((UCCloseableTabItemHeader)this.Header).button_close.Visibility = Visibility.Hidden;
        }

        // Override OnMouseEnter - Show the Close Button
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((UCCloseableTabItemHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }

        // Override OnMouseLeave - Hide the Close Button (If it is NOT selected)
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((UCCloseableTabItemHeader)this.Header).button_close.Visibility = Visibility.Hidden;
            }
        }
        //------------

        // Button MouseEnter - When the mouse is over the button - change color to Red
        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((UCCloseableTabItemHeader)this.Header).button_close.Foreground = Brushes.Red;
        }

        // Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((UCCloseableTabItemHeader)this.Header).button_close.Foreground = Brushes.Black;
        }

        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        void button_close_Click(object sender, RoutedEventArgs e)
        {
            //((TabControl)this.Parent).Items.Remove(this);
            closeTabEvent?.Invoke(this, new EventArgs());
        }

        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((UCCloseableTabItemHeader)this.Header).button_close.Margin = new Thickness(
               ((UCCloseableTabItemHeader)this.Header).label_TabTitle.ActualWidth + 5, 1, 4, 0);
        }

    }
}
