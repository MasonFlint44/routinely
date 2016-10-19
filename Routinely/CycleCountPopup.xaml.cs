using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
