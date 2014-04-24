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
        private readonly WriteableBitmap _bitmap;
        
        private Device _device;
        private VideoStream _stream;

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

        private void StartClicked(object sender, EventArgs e)
        {
            var deviceInfo = _openni.Devices.First();
            _device = deviceInfo.Open();

            _stream = _device.OpenColorStream();
            _stream.Optional.Mirroring = _mirror.IsChecked.Value;

            _stream.Start();
            _stream.NewFrame += _stream_NewFrame;
        }

        private void _stream_NewFrame(VideoStream stream, Frame frame)
        {
            Dispatcher.Invoke(() => frame.Update(_bitmap));
        }

        private void StopClicked(object sender, EventArgs e)
        {
            _stream.NewFrame -= _stream_NewFrame;
            _stream.Stop();
            _stream.Close();

            _device.Close();
        }
    }
}
