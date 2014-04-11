using Nimble;
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

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _openni.Shutdown();
        }

        private void ListButtonClicked(object sender, System.EventArgs e)
        {
            var devies = _openni.Devices;
            foreach (var d in devies)
                System.Console.WriteLine(d.Name);
        }
    }
}
