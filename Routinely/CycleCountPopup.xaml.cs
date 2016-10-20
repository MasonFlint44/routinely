using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Routinely
{
    /// <summary>
    /// Interaction logic for CycleCountPopup.xaml
    /// </summary>
    public partial class CycleCountPopup : UserControl
    {
        private bool _isIndefinite;

        public EventHandler RunClick;
        public bool IsIndefinite
        {
            get { return _isIndefinite; }
            set
            {
                _isIndefinite = value;
                if(value == true)
                {
                    CounterBox.TextBoxSelected = false;
                    IndefiniteButton.Background = Brushes.Gray;
                }
                else
                {
                    CounterBox.TextBoxSelected = true;
                    IndefiniteButton.Background = Brushes.Transparent;
                }
            }
        }

        public int CycleCount
        {
            get { return CounterBox.CounterValue; }
            set { CounterBox.CounterValue = value; }
        }

        public CycleCountPopup()
        {
            InitializeComponent();

            CounterBox.DefaultValue = 1;
            CounterBox.MinusButton.Click += CounterButton_Click;
            CounterBox.PlusButton.Click += CounterButton_Click;
            CounterBox.CounterTextBox.GotKeyboardFocus += CounterButton_Click;
            IndefiniteButton.Click += IndefiniteButton_Click;

            IsIndefinite = false;
        }

        private void IndefiniteButton_Click(object sender, RoutedEventArgs e)
        {
            IsIndefinite = true;
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            IsIndefinite = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = false;

            if(RunClick != null)
            {
                RunClick(this, new EventArgs());
            }
        }
    }
}
