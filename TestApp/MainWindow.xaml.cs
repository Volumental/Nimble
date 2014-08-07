using Nimble;
using Nimble.Native;
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
        private readonly WriteableBitmap _depthBitmap;
        
        private Device _device;
        private VideoStream _stream;
        private VideoStream _depthStream;

        public MainWindow()
        {
            InitializeComponent();
            _bitmap = new WriteableBitmap(640, 480, 72, 72, PixelFormats.Rgb24, null);
            _rgbImage.Source = _bitmap;

            _depthBitmap = new WriteableBitmap(320, 240, 72, 72, PixelFormats.Gray16, null);
            _depthImage.Source = _depthBitmap;
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

            Console.WriteLine("before: " + _stream.VideoMode.Width);
            var vms = _stream.SupportedVideoModes;
            foreach (var videoMode in vms)
            {
                Console.WriteLine("width: " + videoMode.Width + ", height: " + videoMode.Height);
            }
            var vm = vms.First(x => x.PixelFormat == Nimble.PixelFormat.Rgb888 && x.Width == 640);
            _stream.VideoMode = vm;

            _stream.Optional.Mirroring = _mirror.IsChecked.Value;

            GC.WaitForFullGCComplete();

            _stream.Start();
            _stream.NewFrame += _stream_NewFrame;

            // open depth stream
            _depthStream = _device.OpenDepthStream();
            _depthStream.Optional.Mirroring = _mirror.IsChecked.Value;
            _depthStream.Start();
            _depthStream.NewFrame += _stream_NewDepthFrame;

            _device.ImageRegistration = _register.IsChecked.Value ? ImageRegistrationMode.DepthToColor : ImageRegistrationMode.None;
            
        }

        private void _stream_NewFrame(VideoStream stream, Frame frame)
        {
            Dispatcher.Invoke(new Action(() => frame.UpdateAndDispose(_bitmap)));
        }

        private void _stream_NewDepthFrame(VideoStream stream, Frame frame)
        {
            Dispatcher.Invoke(new Action(() => frame.UpdateAndDispose(_depthBitmap)));
        }

        private void StopClicked(object sender, EventArgs e)
        {   
            _stream.Stop();
            _stream.NewFrame -= _stream_NewFrame;
            _stream.Close();

            _depthStream.Stop();
            _depthStream.NewFrame -= _stream_NewDepthFrame;
            _depthStream.Close();

            _device.Close();
        }
    }
}
