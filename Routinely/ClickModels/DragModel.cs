using Routinely;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Routinely.ClickModels
{
    public class DragModel : INotifyPropertyChanged
    {
        private ClickModel _startClickModel;
        private ClickModel _endClickModel;
        private Line _dragPath;

        public readonly double LineThickness = 2;

        public DragModel(int startId, Brush startFill, Brush startBorder, int endId, Brush endFill, Brush endBorder)
        {
            _startClickModel = new ClickModel(startId, startFill, startBorder, this);
            _startClickModel.Settings.ClickType = ClickType.DragStart;

            Line dragPath = new Line();
            dragPath.DataContext = this;
            dragPath.SetBinding(Shape.StrokeProperty, new Binding("BorderColor")
            {
                Source = _startClickModel,
                Mode = BindingMode.TwoWay
            });
            dragPath.StrokeThickness = LineThickness;
            _dragPath = dragPath;

            _endClickModel = new ClickModel(endId, endFill, endBorder, this);
            _endClickModel.Settings.ClickType = ClickType.DragEnd;
        }

        public XElement WriteToXml()
        {
            XElement startPoint = StartClickModel.WriteToXml();
            XElement endPoint = EndClickModel.WriteToXml();

            return new XElement("DragClick", 
                new XElement("StartPoint", startPoint.Element("Point").Attributes(), startPoint.Element("Point").Elements()), 
                new XElement("EndPoint", endPoint.Element("Point").Attributes(), endPoint.Element("Point").Elements()));
        }

        public ClickModel StartClickModel
        {
            get { return _startClickModel; }
        }

        public ClickModel EndClickModel
        {
            get { return _endClickModel; }
        }

        public Line DragPath
        {
            get { return _dragPath; }
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
