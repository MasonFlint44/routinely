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
    /// Interaction logic for ClickTypeItem.xaml
    /// </summary>
    public partial class ClickTypeItem : UserControl, INotifyPropertyChanged
    {
        private Dictionary<ClickType, string> items = new Dictionary<ClickType, string>();
        private ClickType _selectedValue = ClickType.SingleClick;

        public ClickType SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                _selectedValue = value;
                OnPropertyChanged("SelectedValue");
            }
        }

        public ClickTypeItem()
        {
            InitializeComponent();

            items.Add(ClickType.SingleClick, "Single Click");
            items.Add(ClickType.DoubleClick, "Double Click");
            items.Add(ClickType.RightClick, "Right Click");
            items.Add(ClickType.NoClick, "No Click");

            comboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding("SelectedValue")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
            comboBox.ItemsSource = items;
            comboBox.DisplayMemberPath = "Value";
            comboBox.SelectedValuePath = "Key";
        }

        private void ComboBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
        }

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
