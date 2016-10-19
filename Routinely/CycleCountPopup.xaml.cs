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
