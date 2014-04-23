using Nimble;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenNI _openni;
        private WriteableBitmap _bitmap;

        public MainWindow()
        {
            InitializeComponent();
            _bitmap = new WriteableBitmap(640, 480, 72, 72, PixelFormats.Rgb24, null);
            _image.Source = _bitmap;
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
            stream.Start();
            for (int i = 0; i < 10; i++)
            {
                var frame = stream.ReadFrame();
                frame.Update(_bitmap);
            }
            stream.Stop();
            stream.Close();

            device.Close();
        }
    }
}
