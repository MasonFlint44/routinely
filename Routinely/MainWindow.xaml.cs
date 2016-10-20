using Canyonix.UI.Windows;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Routinely
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DockWindow
    {
        private ScreenOverlay OverlayWindow;
        private CancellationTokenSource cancelRoutineTask;
        private CycleCountPopup runPopup;

        private bool isOverlayVisible
        {
            set
            {
                if (value == true)
                {
                    isEditing = true;
                    ShowButton.Visibility = Visibility.Collapsed;
                    HideButton.Visibility = Visibility.Visible;
                }
                else
                {
                    isEditing = false;
                    ShowButton.Visibility = Visibility.Visible;
                    HideButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool isRunning
        {
            set
            {
                if (value == true)
                {
                    isEditing = false;
                    IsPaused = false;
                    isStopped = false;

                    ShowButton.Visibility = Visibility.Collapsed;
                    HideButton.Visibility = Visibility.Collapsed;
                    RunButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    RunButton.Visibility = Visibility.Visible;
                }
            }
        }

        public bool IsPaused
        {
            set
            {
                if(value == true)
                {
                    isEditing = false;
                    isStopped = false;

                    ResumeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ResumeButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool isEditing
        {
            set
            {
                if(value == true)
                {
                    AddClickButton.Visibility = Visibility.Visible;
                    TextButton.Visibility = Visibility.Visible;
                    DragButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Visible;
                    LoadButton.Visibility = Visibility.Visible;
                    RunButton.Visibility = Visibility.Visible;
                }
                else
                {
                    AddClickButton.Visibility = Visibility.Collapsed;
                    TextButton.Visibility = Visibility.Collapsed;
                    DragButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                    LoadButton.Visibility = Visibility.Collapsed;
                    RunButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool isStopped
        {
            set
            {
                if(value == true)
                {
                    isEditing = true;
                    IsPaused = false;
                    isOverlayVisible = true;
                    isRunning = false;

                    StopButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StopButton.Visibility = Visibility.Visible;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            OverlayWindow = new ScreenOverlay(this);

            Left();
            OverlayWindow.Show();

            isOverlayVisible = true;
            isStopped = true;

            runPopup = new CycleCountPopup();
            runPopup.Popup.PlacementTarget = RunButton;
            runPopup.RunClick += RunHandler;

            HotkeyManager.Current.AddOrReplace("StopHotkey", Key.X, ModifierKeys.Control | ModifierKeys.Alt , StopHotkeyHandler);
            HotkeyManager.Current.AddOrReplace("ResumeHotkey", Key.Z, ModifierKeys.Control | ModifierKeys.Alt, ResumeHokeyHandler);
        }

        private void StopHotkeyHandler(object sender, HotkeyEventArgs e)
        {
            OverlayWindow.IsPaused = false;
            runPopup.IsIndefinite = false;
            cancelRoutineTask.Cancel();
        }

        private void ResumeHokeyHandler(object sender, HotkeyEventArgs e)
        {
            if(OverlayWindow.IsPaused == true)
            {
                OverlayWindow.IsPaused = false;
            }
        }

        private void VisibilityHandler()
        {
            if(Permissions.CanPin == true)
            {
                PinButton.Visibility = Visibility.Visible;
            }
            else
            {
                PinButton.Visibility = Visibility.Collapsed;
            }

            if (Permissions.CanUnpin == true)
            {
                UnpinButton.Visibility = Visibility.Visible;
            }
            else
            {
                UnpinButton.Visibility = Visibility.Collapsed;
            }
        }

        private new void Left()
        {
            Dock(DockingEdge.Left);
            VisibilityHandler();
        }

        private void Pin_OnClick(object sender, RoutedEventArgs e)
        {
            Pin();
            VisibilityHandler();
        }

        private void Unpin_OnClick(object sender, RoutedEventArgs e)
        {
            Unpin();
            VisibilityHandler();
        }

        private void Show_OnClick(object sender, RoutedEventArgs e)
        {
            isOverlayVisible = true;
            OverlayWindow.Show();
        }

        private void Hide_OnClick(object sender, RoutedEventArgs e)
        {
            isOverlayVisible = false;
            OverlayWindow.Hide();
        }

        private void AddClick_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.AddClickPoint();
        }

        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            cancelRoutineTask = new CancellationTokenSource();
            runPopup.CycleCount = 1;
            runPopup.Popup.IsOpen = true;
        }

        private void Resume_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.IsPaused = false;
        }

        private async void RunHandler(object sender, EventArgs e)
        {
            isRunning = true;
            OverlayWindow.Hide();
            await Task.Run(() =>
            {
                try
                {
                    if(runPopup.IsIndefinite == true)
                    {
                        while (runPopup.IsIndefinite == true)
                        {
                            OverlayWindow.RunRoutine(runPopup.CycleCount, cancelRoutineTask.Token);
                        }
                    }
                    else
                    {
                        OverlayWindow.RunRoutine(runPopup.CycleCount, cancelRoutineTask.Token);
                    }
                }
                finally
                {
                    Dispatcher.Invoke(() => 
                    {
                        OverlayWindow.Show();
                        cancelRoutineTask.Dispose();

                        isOverlayVisible = true;
                        isStopped = true;
                    });
                }
            });
        }

        private void Text_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.AddTextPoint();
        }

        private void Drag_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.AddDragPoint();
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.SaveRoutine();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.LoadRoutine();
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayWindow.IsPaused = false;
            runPopup.IsIndefinite = false;
            cancelRoutineTask.Cancel();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Unpin();
            OverlayWindow.Close();
            Close();
        }
    }
}
