using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using Routinely.ClickModels;
using System.Xml.Linq;
using Microsoft.Win32;
using System.IO;
using WindowsInput;
using System.Windows.Threading;

namespace Routinely
{
    /// <summary>
    /// Interaction logic for ScreenOverlay.xaml
    /// </summary>
    public partial class ScreenOverlay : Window
    {
        private Brush[] colorArray;
        private Brush[] borderArray;
        private int pointCount = 0;
        private bool isCaptured = false;
        private bool isAdding = false;
        private bool isDragPoint = false;
        private DragModel capturedDrag;
        private readonly Brush tinted = (Brush)new BrushConverter().ConvertFromString("#28FFFFFF");
        private string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Routinely";
        private Window parent;

        public volatile bool IsPaused;

        public ScreenOverlay(Window parent)
        {
            InitializeComponent();
            this.parent = parent;
            BrushConverter hexToBrush = new BrushConverter();

            colorArray = new Brush[5];
            colorArray[0] = (Brush)hexToBrush.ConvertFromString("#FFFF4843");
            colorArray[1] = (Brush)hexToBrush.ConvertFromString("#FFFFBF43");
            colorArray[2] = (Brush)hexToBrush.ConvertFromString("#FF79C975");
            colorArray[3] = (Brush)hexToBrush.ConvertFromString("#FF328BDC");
            colorArray[4] = (Brush)hexToBrush.ConvertFromString("#FF6A3072");

            borderArray = new Brush[5];
            borderArray[0] = (Brush)hexToBrush.ConvertFromString("#FFDE3F3A");
            borderArray[1] = (Brush)hexToBrush.ConvertFromString("#FFDEA63A");
            borderArray[2] = (Brush)hexToBrush.ConvertFromString("#FF69AF66");
            borderArray[3] = (Brush)hexToBrush.ConvertFromString("#FF2C79C0");
            borderArray[4] = (Brush)hexToBrush.ConvertFromString("#FF5C2A63");

            Directory.CreateDirectory(defaultPath);
        }

        public void RunRoutine(int cycleCount, CancellationToken cancelToken)
        {
            InputSimulator input = new InputSimulator();

            try
            {
                List<ClickModel> clickModels = Dispatcher.Invoke(() => GetClickModelsFromCanvas(OverlayCanvas));

                for(int i = 0; i < cycleCount; i++)
                {
                    foreach (ClickModel clickModel in clickModels)
                    {
                        Point point = GetClickModelCoordinates(clickModel);

                        Win32.SetCursorPos((int)point.X, (int)point.Y);
                        Click(clickModel.Settings.ClickType, input);

                        if (clickModel.Parent is TextModel)
                        {
                            if(((TextModel)clickModel.Parent).Text != null)
                            {
                                input.Keyboard.TextEntry(((TextModel)clickModel.Parent).Text);
                            }
                        }

                        if (clickModel.Settings.IndefiniteDelay == false)
                        {
                            Thread.Sleep(clickModel.Settings.Delay);
                        }
                        else
                        {
                            input.Mouse.LeftButtonUp();
                            IsPaused = true;
                            Dispatcher.Invoke(() => { ((MainWindow)parent).IsPaused = true; });
                            while (IsPaused == true)
                            {
                                Thread.Sleep(250);
                            }
                        }
                        cancelToken.ThrowIfCancellationRequested();
                    }
                }
            }
            catch(OperationCanceledException)
            {
                input.Mouse.LeftButtonUp();
                return;
            }
        }

        private void Click(ClickType clickType, InputSimulator input)
        {
            switch (clickType)
            {
                case ClickType.SingleClick:
                    input.Mouse.LeftButtonClick();
                    break;
                case ClickType.DoubleClick:
                    input.Mouse.LeftButtonDoubleClick();
                    break;
                case ClickType.RightClick:
                    input.Mouse.RightButtonClick();
                    break;
                case ClickType.NoClick:
                    break;
                case ClickType.DragStart:
                    input.Mouse.LeftButtonDown();
                    break;
                case ClickType.DragEnd:
                    input.Mouse.LeftButtonUp();
                    break;
            }
        }

        private Point GetClickModelCoordinates(ClickModel clickModel)
        {
            Point point = new Point();
            point.X = (Dispatcher.Invoke(() => Canvas.GetLeft(clickModel.ClickPoint)) + Dispatcher.Invoke(() => clickModel.ClickPoint.Width) / 2);
            point.Y = (Dispatcher.Invoke(() => Canvas.GetTop(clickModel.ClickPoint)) + Dispatcher.Invoke(() => clickModel.ClickPoint.Height) / 2);
            return point;
        }

        private List<ClickModel> GetClickModelsFromCanvas(Canvas canvas)
        {
            List<ClickModel> clickModels = new List<ClickModel>();

            foreach(UIElement child in canvas.Children)
            {
                if (child is Border)
                {
                    if (((Border)child).DataContext is ClickModel)
                    {
                        clickModels.Add((ClickModel)((Border)child).DataContext);
                    }
                }
            }

            return clickModels;
        }

        public void SaveRoutine()
        {
            XDocument routine = new XDocument(new XElement("Routine"));

            foreach(UIElement child in OverlayCanvas.Children)
            {
                if(child is Border)
                {
                    if(((Border)child).DataContext is ClickModel)
                    {
                        ClickModel clickModel = (ClickModel)((Border)child).DataContext;

                        if (clickModel.Parent == null)
                        {
                            routine.Root.Add(clickModel.WriteToXml());
                        }
                        else if(clickModel.Parent is TextModel)
                        {
                            routine.Root.Add(((TextModel)clickModel.Parent).WriteToXml());
                        }
                        else if(clickModel.Parent is DragModel)
                        {
                            if (clickModel.Settings.ClickType == ClickType.DragStart)
                            {
                                routine.Root.Add(((DragModel)clickModel.Parent).WriteToXml());
                            }
                        }
                    }
                }
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save Routine";
            saveDialog.Filter = "XML File (.xml)|*.xml";
            saveDialog.DefaultExt = ".xml";
            saveDialog.ValidateNames = true;
            saveDialog.InitialDirectory = defaultPath;

            if(saveDialog.ShowDialog() == true)
            {
                string filepath = saveDialog.FileName;
                //routine.Root.Add(new XAttribute("name", System.IO.Path.GetFileNameWithoutExtension(filepath)));
                routine.Save(filepath);
            }
        }

        public void LoadRoutine()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open Routine";
            openDialog.Filter = "XML File (.xml)|*.xml";
            openDialog.DefaultExt = ".xml";
            openDialog.ValidateNames = true;
            openDialog.InitialDirectory = defaultPath;

            if(openDialog.ShowDialog() == true)
            {
                OverlayCanvas.Children.Clear();
                XDocument routine = XDocument.Load(openDialog.FileName);

                foreach(XElement element in routine.Root.Elements())
                {
                    if(element.Name == "Click")
                    {
                        XElement point = element.Element("Point");
                        int id = int.Parse(point.Attribute("id").Value);
                        int x = int.Parse(point.Attribute("x").Value);
                        int y = int.Parse(point.Attribute("y").Value);

                        XElement settings = point.Element("Settings");
                        ClickType clickType = (ClickType)Enum.Parse(typeof(ClickType), settings.Attribute("ClickType").Value);
                        int delay = int.Parse(settings.Attribute("Delay").Value);
                        bool indefinite = bool.Parse(settings.Attribute("IndefiniteDelay").Value);

                        ClickModel click = new ClickModel(id, GetColor(id - 1), GetBorderColor(id - 1));
                        click.Settings.ClickType = clickType;
                        click.Settings.Delay = delay;
                        click.Settings.IndefiniteDelay = indefinite;
                        AddClickPointToCanvas(click.ClickPoint, x, y);
                    }
                    else if(element.Name == "TextClick")
                    {
                        string text;
                        if(element.Attribute("text").Value == ":EMPTY:")
                        {
                            text = "";
                        }
                        else
                        {
                            text = element.Attribute("text").Value;
                        }

                        XElement point = element.Element("Point");
                        int id = int.Parse(point.Attribute("id").Value);
                        int x = int.Parse(point.Attribute("x").Value);
                        int y = int.Parse(point.Attribute("y").Value);

                        XElement settings = point.Element("Settings");
                        ClickType clickType = (ClickType)Enum.Parse(typeof(ClickType), settings.Attribute("ClickType").Value);
                        int delay = int.Parse(settings.Attribute("Delay").Value);
                        bool indefinite = bool.Parse(settings.Attribute("IndefiniteDelay").Value);

                        TextModel textClick = new TextModel(id, GetColor(id - 1), GetBorderColor(id - 1));
                        textClick.Text = text;
                        textClick.ClickModel.Settings.ClickType = clickType;
                        textClick.ClickModel.Settings.Delay = delay;
                        textClick.ClickModel.Settings.IndefiniteDelay = indefinite;

                        OverlayCanvas.Children.Add(textClick.TextBox);
                        Canvas.SetLeft(textClick.TextBox, x);
                        Canvas.SetTop(textClick.TextBox, y);
                        AddClickPointToCanvas(textClick.ClickModel.ClickPoint, x, y);
                    }
                    else if(element.Name == "DragClick")
                    {
                        XElement startPoint = element.Element("StartPoint");
                        int startId = int.Parse(startPoint.Attribute("id").Value);
                        int startX = int.Parse(startPoint.Attribute("x").Value);
                        int startY = int.Parse(startPoint.Attribute("y").Value);

                        XElement startSettings = startPoint.Element("Settings");
                        ClickType startClickType = (ClickType)Enum.Parse(typeof(ClickType), startSettings.Attribute("ClickType").Value);
                        int startDelay = int.Parse(startSettings.Attribute("Delay").Value);
                        bool startIndefinite = bool.Parse(startSettings.Attribute("IndefiniteDelay").Value);

                        XElement endPoint = element.Element("EndPoint");
                        int endId = int.Parse(endPoint.Attribute("id").Value);
                        int endX = int.Parse(endPoint.Attribute("x").Value);
                        int endY = int.Parse(endPoint.Attribute("y").Value);

                        XElement endSettings = endPoint.Element("Settings");
                        ClickType endClickType = (ClickType)Enum.Parse(typeof(ClickType), endSettings.Attribute("ClickType").Value);
                        int endDelay = int.Parse(endSettings.Attribute("Delay").Value);
                        bool endIndefinite = bool.Parse(endSettings.Attribute("IndefiniteDelay").Value);

                        DragModel dragModel = new DragModel(startId, GetColor(startId - 1), GetBorderColor(startId - 1), endId, GetColor(endId - 1), GetBorderColor(endId - 1));
                        dragModel.StartClickModel.Settings.ClickType = startClickType;
                        dragModel.StartClickModel.Settings.Delay = startDelay;
                        dragModel.StartClickModel.Settings.IndefiniteDelay = startIndefinite;
                        dragModel.EndClickModel.Settings.ClickType = endClickType;
                        dragModel.EndClickModel.Settings.Delay = endDelay;
                        dragModel.EndClickModel.Settings.IndefiniteDelay = endIndefinite;

                        OverlayCanvas.Children.Add(dragModel.DragPath);
                        dragModel.DragPath.X1 = startX;
                        dragModel.DragPath.Y1 = startY;
                        dragModel.DragPath.X2 = endX;
                        dragModel.DragPath.Y2 = endY;
                        AddClickPointToCanvas(dragModel.StartClickModel.ClickPoint, startX, startY);
                        AddClickPointToCanvas(dragModel.EndClickModel.ClickPoint, endX, endY);
                    }
                }
            }  
        }

        private Brush GetColor(int id)
        {
            return colorArray[id % colorArray.Count()];
        }

        private Brush GetBorderColor(int id)
        {
            return borderArray[id % borderArray.Count()];
        }

        public void AddClickPoint()
        {
            OverlayCanvas.Background = tinted;
            MouseLeftButtonDown += AddClickPoint_MouseLeftButtonDown;
            isAdding = true;
        }

        public void AddTextPoint()
        {
            OverlayCanvas.Background = tinted;
            MouseLeftButtonDown += AddTextPoint_MouseLeftButtonDown;
            isAdding = true;
        }

        public void AddDragPoint()
        {
            OverlayCanvas.Background = tinted;
            MouseLeftButtonDown += AddDragPoint_MouseLeftButtonDown;
            isAdding = true;
        }

        private void AddClickPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickModel clickModel = new ClickModel(pointCount + 1, GetColor(pointCount), GetColor(pointCount));

            AddClickPointToCanvas(clickModel.ClickPoint, e.GetPosition(OverlayCanvas).X, e.GetPosition(OverlayCanvas).Y);
            
            OverlayCanvas.Background = Brushes.Transparent;
            MouseLeftButtonDown -= AddClickPoint_MouseLeftButtonDown;
            isAdding = false;
        }

        private void AddTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextModel textModel = new TextModel(pointCount + 1, GetColor(pointCount), GetBorderColor(pointCount));

            OverlayCanvas.Children.Add(textModel.TextBox);
            Canvas.SetLeft(textModel.TextBox, e.GetPosition(OverlayCanvas).X);
            Canvas.SetTop(textModel.TextBox, e.GetPosition(OverlayCanvas).Y);

            AddClickPointToCanvas(textModel.ClickModel.ClickPoint, e.GetPosition(OverlayCanvas).X, e.GetPosition(OverlayCanvas).Y);

            OverlayCanvas.Background = Brushes.Transparent;
            MouseLeftButtonDown -= AddTextPoint_MouseLeftButtonDown;
            isAdding = false;
        }

        private void AddDragPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point coordinates = new Point(e.GetPosition(OverlayCanvas).X, e.GetPosition(OverlayCanvas).Y);

            if (isDragPoint == false)
            {
                isDragPoint = true;

                DragModel dragModel = new DragModel(pointCount + 1, GetColor(pointCount), GetBorderColor(pointCount),
                    pointCount + 2, GetColor(pointCount + 1), GetBorderColor(pointCount + 1));

                OverlayCanvas.Children.Add(dragModel.DragPath);
                dragModel.DragPath.X1 = dragModel.DragPath.X2 = coordinates.X;
                dragModel.DragPath.Y1 = dragModel.DragPath.Y2 = coordinates.Y;

                AddClickPointToCanvas(dragModel.StartClickModel.ClickPoint, coordinates.X, coordinates.Y);

                Mouse.Capture((UIElement)sender);
                isCaptured = true;
                capturedDrag = dragModel;
                MouseMove += AddDragPoint_MouseMove;
            }
            else
            {
                isDragPoint = false;

                AddClickPointToCanvas(capturedDrag.EndClickModel.ClickPoint, coordinates.X, coordinates.Y);

                Mouse.Capture(null);
                capturedDrag = null;
                isCaptured = false;
                MouseMove -= AddDragPoint_MouseMove;

                OverlayCanvas.Background = Brushes.Transparent;
                MouseLeftButtonDown -= AddDragPoint_MouseLeftButtonDown;
                isAdding = false;
            }
        }

        private void AddDragPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if(isDragPoint == true)
            {
                capturedDrag.DragPath.X2 = e.GetPosition(OverlayCanvas).X;
                capturedDrag.DragPath.Y2 = e.GetPosition(OverlayCanvas).Y;
            }
        }

        private void AddClickPointToCanvas(Border clickPoint, double clickX, double clickY)
        {
            clickPoint.MouseLeftButtonDown += Point_MouseLeftButtonDown;
            clickPoint.MouseMove += Point_MouseMove;
            clickPoint.MouseLeftButtonUp += Point_MouseLeftButtonUp;
            ((ClickModel)clickPoint.DataContext).Settings.Remove += Point_Remove;

            OverlayCanvas.Children.Add(clickPoint);
            Canvas.SetLeft(clickPoint, clickX - (clickPoint.Width / 2));
            Canvas.SetTop(clickPoint, clickY - (clickPoint.Height / 2));

            pointCount++;
        }

        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isAdding == false)
            {
                Mouse.Capture((UIElement)sender);
                isCaptured = true;
            }
        }

        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCaptured == true)
            {
                ClickModel senderModel = (ClickModel)((FrameworkElement)sender).DataContext;
                Point coordinates = new Point(e.GetPosition(OverlayCanvas).X, e.GetPosition(OverlayCanvas).Y);

                Canvas.SetLeft(senderModel.ClickPoint, coordinates.X - (senderModel.ClickPoint.Width / 2));
                Canvas.SetTop(senderModel.ClickPoint, coordinates.Y - (senderModel.ClickPoint.Height / 2));

                if (senderModel.Parent is TextModel)
                {
                    TextModel senderTextModel = (TextModel)senderModel.Parent;

                    Canvas.SetLeft(senderTextModel.TextBox, coordinates.X);
                    Canvas.SetTop(senderTextModel.TextBox, coordinates.Y);
                }
                else if (senderModel.Parent is DragModel)
                {
                    DragModel senderDragModel = (DragModel)senderModel.Parent;

                    if (sender == senderDragModel.StartClickModel.ClickPoint)
                    {
                        senderDragModel.DragPath.X1 = coordinates.X;
                        senderDragModel.DragPath.Y1 = coordinates.Y;
                    }
                    else if (sender == senderDragModel.EndClickModel.ClickPoint)
                    {
                        senderDragModel.DragPath.X2 = coordinates.X;
                        senderDragModel.DragPath.Y2 = coordinates.Y;
                    }
                }
            }
        }

        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            isCaptured = false;
        }

        private void Point_Remove(object sender, EventArgs e)
        {
            int numRemoved = 0;
            ClickModel senderModel = ((ClickModelSettings)sender).Parent;

            if (senderModel.Parent == null)
            {
                OverlayCanvas.Children.Remove(senderModel.ClickPoint);
                pointCount--;
                numRemoved = 1;
            }
            else if (senderModel.Parent is TextModel)
            {
                TextModel senderTextModel = (TextModel)senderModel.Parent;

                OverlayCanvas.Children.Remove(senderTextModel.ClickModel.ClickPoint);
                OverlayCanvas.Children.Remove(senderTextModel.TextBox);

                pointCount--;
                numRemoved = 1;
            }
            else if (senderModel.Parent is DragModel)
            {
                DragModel senderDragModel = (DragModel)senderModel.Parent;

                OverlayCanvas.Children.Remove(senderDragModel.EndClickModel.ClickPoint);
                OverlayCanvas.Children.Remove(senderDragModel.StartClickModel.ClickPoint);
                OverlayCanvas.Children.Remove(senderDragModel.DragPath);

                pointCount -= 2;
                numRemoved = 2;
            }

            foreach (FrameworkElement child in OverlayCanvas.Children)
            {
                if (child.DataContext is ClickModel)
                {
                    ClickModel childModel = (ClickModel)child.DataContext;

                    if (childModel.Id > senderModel.Id)
                    {
                        childModel.Id -= numRemoved;
                        childModel.FillColor = GetColor(childModel.Id - 1);
                        childModel.BorderColor = GetBorderColor(childModel.Id - 1);
                    }
                }
            }
        }
    }
}
