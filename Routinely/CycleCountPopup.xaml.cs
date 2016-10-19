using Routinely.ClickModels.ContextMenuElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for CycleCountPopup.xaml
    /// </summary>
    public partial class CycleCountPopup : UserControl
    {
        public EventHandler RunClick;

        public CycleCountPopup()
        {
            InitializeComponent();

            CounterBox.DefaultValue = 1;

            //Stole the counter part from a DelayItem
            //delayItem = new DelayItem();
            //delayItem.HighlightColor = Brushes.Gray;

            //((StackPanel)delayItem.MinusButton.Parent).Children.Remove(delayItem.MinusButton);
            //((StackPanel)delayItem.DelayTextBox.Parent).Children.Remove(delayItem.DelayTextBox);
            //((StackPanel)delayItem.PlusButton.Parent).Children.Remove(delayItem.PlusButton);
            //delayItem.MinusButton.BorderThickness = new Thickness(1);
            //delayItem.PlusButton.BorderThickness = new Thickness(1);
            //delayItem.MinusButton.Width = 30;
            //delayItem.PlusButton.Width = 30;
            //delayItem.DelayTextBox.BorderThickness = new Thickness(1);
            //delayItem.MinusButton.Interval = 33;
            //delayItem.PlusButton.Interval = 33;
            //delayItem.MinusButton.ToolTip = "Decrease number of cycles";
            //delayItem.PlusButton.ToolTip = "Increase number of cycles";

            //StackPanel.Children.Add(delayItem.MinusButton);
            //StackPanel.Children.Add(delayItem.DelayTextBox);
            //StackPanel.Children.Add(delayItem.PlusButton);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = false;

            if(RunClick != null)
            {
                RunClick(this, new EventArgs());
            }
        }


        public int CycleCount
        {
            get { return CounterBox.CounterValue; }
            set { CounterBox.CounterValue = value; }
        }
    }
}
