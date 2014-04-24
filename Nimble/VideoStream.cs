using Nimble.Native;
using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Nimble
{
    public class VideoStream
    {
        private readonly IntPtr _handle;
        private event Action<VideoStream, Frame> _onNewFrame;

        private IntPtr _callbackHandle;
        private int _subscribers = 0;
        private readonly OniNewFrameCallback _callback;

        internal VideoStream(Device device, OniSensorType sensorType)
        {
            var status = OpenNI2.oniDeviceCreateStream(device.Handle, sensorType, out _handle);
            status.ThrowIfFailed();
            _callback = new OniNewFrameCallback(NewFrameCallback);
        }

        private void NewFrameCallback(IntPtr streamHandle, IntPtr cookie)
        {
            var tmp = _onNewFrame;
            var frame = ReadFrame();
            if (tmp != null) tmp(this, frame);
        }

        public event Action<VideoStream, Frame> NewFrame
        {
            add
            {
                _onNewFrame += value;
                if (_subscribers == 0)
                {
                    var status = OpenNI2.oniStreamRegisterNewFrameCallback(_handle, _callback, IntPtr.Zero, out _callbackHandle);
                    status.ThrowIfFailed();
                }
                _subscribers++;
            }
            remove
            {
                _onNewFrame -= value;
                _subscribers--;
                if (_subscribers == 0)
                {
                    OpenNI2.oniStreamUnregisterNewFrameCallback(_handle, _callbackHandle);
                }
            }
        }

        public void Start()
        {
            var status = OpenNI2.oniStreamStart(_handle);
            status.ThrowIfFailed();
        }

        public Frame ReadFrame()
        {
            IntPtr framePointer = IntPtr.Zero;
            var status = OpenNI2.oniStreamReadFrame(_handle, out framePointer);
            status.ThrowIfFailed();
            var frame = (OniFrame)Marshal.PtrToStructure(framePointer, typeof(OniFrame));
            return Frame.From(frame);
        }

        public void Stop()
        {
            OpenNI2.oniStreamStop(_handle);
        }

        public void Close()
        {
            OpenNI2.oniStreamDestroy(_handle);
        }
    }
}
