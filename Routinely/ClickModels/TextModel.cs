using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace Routinely.ClickModels
{
    public class TextModel : INotifyPropertyChanged
    {
        private string _text;
        private Border _textBox;
        private ClickModel _clickModel;

        public readonly double Width = 50;
        public readonly double Height = 25;
        public readonly CornerRadius CornerRadius = new CornerRadius(5);
        public readonly Brush FillColor = Brushes.SlateGray;
        public readonly Brush BorderColor = Brushes.DarkSlateGray;
        public readonly Thickness BorderThickness = new Thickness(3);
        public readonly double FontSize = 12;
        public readonly Brush FontColor = Brushes.White;

        public TextModel(int id, Brush fillColor, Brush borderColor)
        {
            _clickModel = new ClickModel(id, fillColor, borderColor, this);

            Border border = new Border();

            border.Width = Width;
            border.Height = Height;
            border.CornerRadius = CornerRadius;
            border.Background = FillColor;
            border.BorderBrush = BorderColor;
            border.BorderThickness = BorderThickness;

            TextBox textBox = new TextBox();
            textBox.FontSize = FontSize;
            textBox.Foreground = FontColor;
            textBox.CaretBrush = FontColor;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = new Thickness(0);
            textBox.AcceptsReturn = true;
            textBox.ContextMenu = null;
            textBox.DataContext = this;

            textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, new Binding("Text")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            border.Child = textBox;

            _textBox = border;
        }

        public XElement WriteToXml()
        {
            XElement clickModel = ClickModel.WriteToXml();

            return new XElement("TextClick", new XAttribute("text", Text ?? ":EMPTY:"), clickModel.Elements());
        }

        public Border TextBox
        {
            get { return _textBox; }
        }

        public ClickModel ClickModel
        {
            get { return _clickModel; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
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
