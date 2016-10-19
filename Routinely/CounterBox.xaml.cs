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

namespace Routinely
{
    /// <summary>
    /// Interaction logic for CounterBox.xaml
    /// </summary>
    public partial class CounterBox : UserControl, INotifyPropertyChanged
    {
        private int _counterValue;
        private string _counterText;
        private int _defaultValue = 0;
        private int _minValue = 0;
        private int _maxValue = int.MaxValue;
        private bool _textBoxSelected;

        public Brush HighlightColor = Brushes.Gray;
        
        public bool TextBoxSelected
        {
            get { return _textBoxSelected; }
            set
            {
                _textBoxSelected = value;
                if(value == true)
                {
                    CounterTextBox.Background = HighlightColor;
                }
                else
                {
                    CounterTextBox.Background = Brushes.Transparent;
                }
            }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                if(CounterValue > value)
                {
                    CounterValue = value;
                }
                _maxValue = value;
            }
        }

        public int MinValue
        {
            get { return _minValue; }
            set
            {
                if (CounterValue < value)
                {
                    CounterValue = value;
                }
                _minValue = value; 
            }
        }

        public int DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                CounterValue = value;
            } 
        }

        public int CounterValue
        {
            get { return _counterValue; }
            set
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    _counterValue = value;
                    CounterText = value.ToString();
                }
            }
        }

        public string CounterText
        {
            get { return _counterText; }
            set
            {
                int hold;
                if (int.TryParse(value, out hold))
                {
                    _counterText = value;
                    _counterValue = hold;
                    OnPropertyChanged("CounterText");
                }
                else
                {
                    CounterValue = DefaultValue;
                }
            }
        }

        public CounterBox()
        {
            InitializeComponent();

            CounterValue = DefaultValue;

            CounterTextBox.SetBinding(TextBox.TextProperty, new Binding("CounterText")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
            CounterTextBox.Text = CounterValue.ToString();
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            CounterValue--;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            CounterValue++;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Control)sender).Background = HighlightColor;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Control)sender).Background = Brushes.Transparent;
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Control)sender).Background = HighlightColor;
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if(TextBoxSelected == true)
            {
                ((Control)sender).Background = HighlightColor;
            }
            else
            {
                ((Control)sender).Background = Brushes.Transparent;
            }
        }

        //private void Control_MouseLeave(object sender, RoutedEventArgs e)
        //{
        //    MinusButton.Background = Brushes.Transparent;
        //    CounterTextBox.Background = Brushes.Transparent;
        //    PlusButton.Background = Brushes.Transparent;
        //}

        //private void Control_MouseEnter(object sender, RoutedEventArgs e)
        //{
        //    CounterTextBox.Background = Brushes.Gray;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
