using Nimble.Native;

namespace Nimble
{
    public class VideoMode
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _fps;
        private PixelFormat _pixelFormat;

        public VideoMode(int width, int height, int fps, PixelFormat pixelFormat)
        {
            _width = width;
            _height = height;
            _fps = fps;
            _pixelFormat = pixelFormat;
        }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public int Fps { get { return _fps; } }
        public PixelFormat PixelFormat { get { return _pixelFormat; } }

        internal OniVideoMode ToRaw()
        {
            return new OniVideoMode() { resolutionX = _width, resolutionY = _height, fps = _fps, pixelFormat = _pixelFormat.ToRaw() };
        }
    }
}
