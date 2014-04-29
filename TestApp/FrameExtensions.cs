using Nimble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApp
{
    internal static class FrameExtensions
    {
        public static BitmapSource ToBitmapSource(this Frame frame)
        {
            using (var pixels = frame.LockPixels())
            {
                var bitmapSource = BitmapSource.Create(frame.Width, frame.Height, 72, 72,
                    GetPixelFormatFor(pixels), null, pixels.Data, pixels.DataSize, pixels.Stride);
                bitmapSource.Freeze();
                return bitmapSource;
            }
        }

        public static void Update(this Frame frame, WriteableBitmap bitmap)
        {
            var rect = new Int32Rect(0, 0, frame.Width, frame.Height);
            using (var pixels = frame.LockPixels())
            {
                bitmap.WritePixels(rect, pixels.Data, pixels.DataSize, pixels.Stride);
            }
        }

        private static System.Windows.Media.PixelFormat GetPixelFormatFor(Nimble.Frame.Pixels pixels)
        {
            switch (pixels.Format)
            {
                case Nimble.PixelFormat.Depth_1mm: return PixelFormats.Gray16;
                case Nimble.PixelFormat.Rgb888: return PixelFormats.Rgb24;
            }
            throw new ArgumentException("frame", "Only DEPTH_1MM and RGB888 are supported, but not " + pixels.Format);
        }
    }
}
