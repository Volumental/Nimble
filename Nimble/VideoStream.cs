using Nimble.Native;
using System;

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
