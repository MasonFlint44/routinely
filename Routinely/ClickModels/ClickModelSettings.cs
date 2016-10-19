using System;
using System.Windows.Media;

namespace Routinely.ClickModels
{
    public enum ClickType
    {
        SingleClick,
        DoubleClick,
        RightClick,
        NoClick,
        DragStart,
        DragEnd
    }

    public class ClickModelSettings
    {
        public ClickContextMenu ContextMenu;
        public ClickModel Parent;
        public event EventHandler Remove;

        public bool IndefiniteDelay
        {
            get { return ContextMenu.DelayItem.IsIndefinite; }
            set
            {
                ContextMenu.DelayItem.IsIndefinite = value;
                if(value == true)
                {
                    ContextMenu.DelayItem.CounterBox.CounterTextBox.Background = Brushes.Transparent;
                    ContextMenu.DelayItem.IndefiniteButton.Background = Brushes.DimGray;
                }
                else if(value == false)
                {
                    ContextMenu.DelayItem.CounterBox.CounterTextBox.Background = Brushes.DimGray;
                    ContextMenu.DelayItem.IndefiniteButton.Background = Brushes.Transparent;
                }
            }
        }

        public int Delay
        {
            get { return ContextMenu.DelayItem.DelayValue; }
            set { ContextMenu.DelayItem.DelayValue = value; }
        }

        public ClickType ClickType
        {
            get { return ContextMenu.ClickTypeItem.SelectedValue; }
            set { ContextMenu.ClickTypeItem.SelectedValue = value; }
        }

        public ClickModelSettings(ClickModel clickModel)
        {
            Parent = clickModel;
            ContextMenu = new ClickContextMenu(this);
        }

        public void OnRemove(EventArgs e)
        {
            if (Remove != null)
            {
                Remove(this, e);
            }
        }
    }
}
