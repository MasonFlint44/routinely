using Routinely.ClickModels;
using Routinely.ClickModels.ContextMenuElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Routinely.ClickModels
{
    public class ClickContextMenu : ContextMenu
    {
        public ClickModelSettings Settings;
        public DelayItem DelayItem = new DelayItem();
        public ClickTypeItem ClickTypeItem = new ClickTypeItem();

        public ClickContextMenu(ClickModelSettings settings)
        {
            Settings = settings;
            Style = (Style)Application.Current.Resources["ClickContextMenu"];

            MenuItem remove = new MenuItem();
            remove.Style = (Style)Application.Current.Resources["ClickMenuItem"];
            remove.Header = "Remove";
            remove.ToolTip = "Remove this point from the routine";
            remove.Click += Remove_Click;
            Items.Add(remove);

            MenuItem delay = new MenuItem();
            delay.Style = (Style)Application.Current.Resources["ClickMenuItem"];
            delay.Header = DelayItem;
            Items.Add(delay);

            if(Settings.Parent.Parent == null)
            {
                MenuItem clickType = new MenuItem();
                clickType.Style = (Style)Application.Current.Resources["ClickMenuItem"];
                clickType.Header = ClickTypeItem;
                clickType.StaysOpenOnClick = true;
                Items.Add(clickType);
            }         
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Settings.OnRemove(e);
        }
    }
}
