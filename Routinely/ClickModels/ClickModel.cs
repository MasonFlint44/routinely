using Routinely.ClickModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace Routinely
{
    public class ClickModel : INotifyPropertyChanged
    {
        private int _id;
        private Brush _fillColor;
        private Brush _borderColor;
        private string _idText;
        private Border _clickPoint;
        private ClickModelSettings _settings;
        private object _parent;

        public readonly double Radius = 12;
        public readonly CornerRadius CornerRadius = new CornerRadius(50);
        public readonly Thickness BorderThickness = new Thickness(2);
        public readonly double FontSize = 8;
        public readonly Brush FontColor = Brushes.White;

        public ClickModel(int id, Brush fillColor, Brush borderColor)
            : this(id, fillColor, borderColor, null)
        {

        }

        public ClickModel(int id, Brush fillColor, Brush borderColor, object parent)
        {
            _parent = parent;
            Id = id;
            FillColor = fillColor;
            BorderColor = borderColor;
            _settings = new ClickModelSettings(this);
            _clickPoint = GetClickPoint();
        }

        public ClickModel(XElement element)
        {
            Id = int.Parse(element.Element("Point").Attribute("id").Value);

        }

        public XElement WriteToXml()
        {
            return new XElement("Click", new XElement("Point", new XAttribute("id", IdText), new XAttribute("x", Canvas.GetLeft(ClickPoint) + ClickPoint.Width / 2), new XAttribute("y", Canvas.GetTop(ClickPoint) + ClickPoint.Height / 2),
                new XElement("Settings", new XAttribute("ClickType", Settings.ClickType), new XAttribute("Delay", Settings.Delay), new XAttribute("IndefiniteDelay", Settings.IndefiniteDelay))));
        }
        
        private Border GetClickPoint()
        {
            Border point = new Border();

            point.DataContext = this;
            point.Width = point.Height = Radius;
            point.CornerRadius = CornerRadius;
            point.BorderThickness = BorderThickness;

            point.ContextMenu = Settings.ContextMenu;

            point.SetBinding(Border.BackgroundProperty, new Binding("FillColor")
            {
                Mode = BindingMode.TwoWay
            });
            point.SetBinding(Border.BorderBrushProperty, new Binding("BorderColor")
            {
                Mode = BindingMode.TwoWay
            });

            TextBlock text = new TextBlock();
            text.FontSize = FontSize;
            text.Foreground = FontColor;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;

            text.SetBinding(TextBlock.TextProperty, new Binding("IdText")
            {
                Mode = BindingMode.TwoWay
            });

            point.Child = text;

            return point;
        }

        public Border ClickPoint
        {
            get { return _clickPoint; }
        }

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                IdText = _id.ToString();
                OnPropertyChanged("Id");
            }
        }

        public Brush FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                OnPropertyChanged("FillColor");
            }
        }

        public Brush BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }

        public string IdText
        {
            get { return _idText; }
            set
            {
                _idText = value;
                OnPropertyChanged("IdText");
            }
        }

        public ClickModelSettings Settings
        {
            get { return _settings; }
        }

        public object Parent
        {
            get { return _parent; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
