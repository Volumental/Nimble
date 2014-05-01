using Nimble.Native;
using System;

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
        
        internal static Frame From(OniFrame frame)
        {
            return new Frame(frame);
        }

        public class Pixels : IDisposable
        {
            private readonly Frame _frame;
            public Pixels(Frame frame) { _frame = frame; }

            public IntPtr Data { get { return _frame._raw.data; } }
            public int DataSize { get { return _frame._raw.dataSize; } }
            public int Stride { get { return _frame._raw.stride; } }
            public PixelFormat Format
            {
                get { return (PixelFormat)((int)_frame._raw.videoMode.pixelFormat); }
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }

        public Pixels LockPixels()
        {
            return new Pixels(this);
        }
    }
}
