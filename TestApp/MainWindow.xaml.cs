using Nimble;
using System;
using System.Linq;
using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenNI _openni;

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _openni = new Nimble.OpenNI();
            _openni.Initialize();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _openni.Shutdown();
        }
        
        private void ListButtonClicked(object sender, EventArgs e)
        {
            var devies = _openni.Devices;
            foreach (var d in devies)
                System.Console.WriteLine(d.Name);
        }

        private void OpenClicked(object sender, EventArgs e)
        {
            var deviceInfo = _openni.Devices.First();
            var device = deviceInfo.Open();

            var stream = device.OpenColorStream();
            stream.Close();

            device.Close();
        }
    }
}
