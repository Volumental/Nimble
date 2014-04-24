using Nimble.Native;
using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Nimble
{
    public class VideoStream
    {
        private readonly IntPtr _handle;

        internal VideoStream(Device device, OniSensorType sensorType)
        {
            var status = OpenNI2.oniDeviceCreateStream(device.Handle, sensorType, out _handle);
            status.ThrowIfFailed();
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
