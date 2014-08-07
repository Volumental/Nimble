using Nimble.Native;
using System;
using System.Runtime.InteropServices;

namespace Nimble
{
    public class Frame: IDisposable
    {
        private readonly IntPtr _framePointer;
        private readonly OniFrame _raw;
        private bool _disposed;

        private Frame(IntPtr framePointer)
        {
            _raw = (OniFrame)Marshal.PtrToStructure(framePointer, typeof(OniFrame));
            _framePointer = framePointer;
        }

        public int Width { get { return _raw.width; } }
        public int Height { get { return _raw.height; } }
        
        internal static Frame From(IntPtr framePointer)
        {
            return new Frame(framePointer);
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
                get { return _frame._raw.videoMode.pixelFormat.ToNimblePixelFormat(); }
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

        public void Dispose()
        {
            Free();
            GC.SuppressFinalize(this);
        }

        ~Frame()
        {
            Free();
        }

        private void Free()
        {
            if (!_disposed)
            {
                OpenNI2.oniFrameRelease(_framePointer);
                _disposed = true;
            }
        }
    }
}
