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

namespace Routinely.ClickModels.ContextMenuElements
{
    /// <summary>
    /// Interaction logic for DelayButton.xaml
    /// </summary>
    public partial class DelayItem : UserControl//, INotifyPropertyChanged
    {
        private bool _isIndefinite;

        public bool IsIndefinite
        {
            get { return _isIndefinite; }
            set
            {
                _isIndefinite = value;
                if(value == true)
                {
                    CounterBox.TextBoxSelected = false;
                    //CounterBox.CounterTextBox.Background = Brushes.Transparent;
                    IndefiniteButton.Background = CounterBox.HighlightColor;
                }
                else
                {
                    CounterBox.TextBoxSelected = true;
                    //CounterBox.CounterTextBox.Background = CounterBox.HighlightColor;
                    IndefiniteButton.Background = Brushes.Transparent;
                }

            }
        }

        public int DelayValue
        {
            get { return CounterBox.CounterValue; }
            set { CounterBox.CounterValue = value; }
        }

        public DelayItem()
        {
            InitializeComponent();
            IsIndefinite = false;

            CounterBox.DefaultValue = 250;
            CounterBox.HighlightColor = Brushes.DimGray;
            CounterBox.MinusButton.BorderThickness = new Thickness(0, 1, 0, 1);
            CounterBox.PlusButton.BorderThickness = new Thickness(0, 1, 0, 1);
            CounterBox.MinusButton.Interval = 10;
            CounterBox.PlusButton.Interval = 10;   
            CounterBox.MinusButton.Click += Button_Click;
            CounterBox.PlusButton.Click += Button_Click;
            CounterBox.CounterTextBox.GotKeyboardFocus += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsIndefinite = false;
            //IndefiniteButton.Background = Brushes.Transparent;
            //CounterBox.CounterTextBox.Background = CounterBox.HighlightColor;
        }

        private void IndefiniteButton_Click(object sender, RoutedEventArgs e)
        {
            IsIndefinite = true;
            //CounterBox.CounterTextBox.Background = Brushes.Transparent;
            //IndefiniteButton.Background = CounterBox.HighlightColor;
        }

        private void IndefiniteButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            IndefiniteButton.Background = CounterBox.HighlightColor;
        }

        private void IndefiniteButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            IndefiniteButton.Background = Brushes.Transparent;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            CounterBox.HighlightColor = Brushes.Gray;
            if(IsIndefinite == true)
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

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            CounterBox.HighlightColor = Brushes.DimGray;
            if (IsIndefinite == true)
            {
                CounterBox.TextBoxSelected = false;
                IndefiniteButton.Background = Brushes.DimGray;
            }
            else
            {
                CounterBox.TextBoxSelected = true;
                IndefiniteButton.Background = Brushes.Transparent;
            }
        }

        //private void Button_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    ((Control)sender).Background = HighlightColor;
        //}

        //private void Button_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    ((Control)sender).Background = Brushes.Transparent;

        //    if (IsIndefinite == true)
        //    {
        //        IndefiniteButton.Background = HighlightColor;
        //        DelayTextBox.Background = Brushes.Transparent;
        //    }
        //    else
        //    {
        //        IndefiniteButton.Background = Brushes.Transparent;
        //        DelayTextBox.Background = HighlightColor;
        //    }
        //}

        //private void Control_MouseLeave(object sender, RoutedEventArgs e)
        //{
        //    MinusButton.Background = Brushes.Transparent;
        //    PlusButton.Background = Brushes.Transparent;

        //    if (IsIndefinite == true)
        //    {
        //        IndefiniteButton.Background = Brushes.Gray;
        //        DelayTextBox.Background = Brushes.Transparent;
        //    }
        //    else
        //    {
        //        IndefiniteButton.Background = Brushes.Transparent;
        //        DelayTextBox.Background = Brushes.Gray;
        //    }
        //}

        //private void Control_MouseEnter(object sender, RoutedEventArgs e)
        //{
        //    if (IsIndefinite == true)
        //    {
        //        IndefiniteButton.Background = HighlightColor;
        //        DelayTextBox.Background = Brushes.Transparent;
        //    }
        //    else
        //    {
        //        IndefiniteButton.Background = Brushes.Transparent;
        //        DelayTextBox.Background = HighlightColor;
        //    }
        //}

        //private void DelayTextBox_Click(object sender, RoutedEventArgs e)
        //{
        //    IsIndefinite = false;
        //    DelayTextBox.Background = HighlightColor;
        //    IndefiniteButton.Background = Brushes.Transparent;
        //}

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
