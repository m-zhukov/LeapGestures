using LeapGR.Impl;
using System;
using System.Windows;

namespace LeapUI
{
    public partial class MainWindow : Window
    {
        GestureProcessor _gp;

        public MainWindow()
        {
            InitializeComponent();

            _gp = new GestureProcessor();
            _gp.GestureRecognized += _gp_GestureRecognized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        void _gp_GestureRecognized(LeapGR.GestureModel.Gesture gesture)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                lblGesture.Content = gesture.GestureName;
            }));
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _gp.GestureRecognized -= _gp_GestureRecognized;
            _gp.UninitializeSensor();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _gp.GestureRecognized -= _gp_GestureRecognized;
            _gp.UninitializeSensor();
        }
    }
}
