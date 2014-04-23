using Nimble.Native;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nimble
{
    public class Frame
    {
        private readonly OniFrame _raw;

        private Frame(OniFrame frame)
        {
            _raw = frame;
        }

        public int Width { get { return _raw.width; } }
        public int Height { get { return _raw.height; } }

        public BitmapSource ToBitmapSource()
        {
            var bitmapSource = BitmapSource.Create(Width, Height, 72, 72,
                GetPixelFormatFor(_raw), null, _raw.data, _raw.dataSize, _raw.stride);
            bitmapSource.Freeze();
            return bitmapSource;
        }

        public void Update(WriteableBitmap bitmap)
        {
            var rect = new Int32Rect(0, 0, Width, Height);
            bitmap.WritePixels(rect, _raw.data, _raw.dataSize, _raw.stride);
        }

        private static PixelFormat GetPixelFormatFor(OniFrame frame)
        {
            if (frame.videoMode.pixelFormat == OniPixelFormat.ONI_PIXEL_FORMAT_DEPTH_1_MM)
            {
                return PixelFormats.Gray16;
            }
            if (frame.videoMode.pixelFormat == OniPixelFormat.ONI_PIXEL_FORMAT_RGB888)
            {
                return PixelFormats.Rgb24;
            }
            throw new ArgumentException("frame", "Only DEPTH_1MM and RGB888 are supported, but not " + frame.videoMode.pixelFormat);
        }

        internal static Frame From(OniFrame frame)
        {
            return new Frame(frame);
        }
    }
}
