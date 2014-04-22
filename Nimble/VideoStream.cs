using Nimble.Native;
using System;

namespace Nimble
{
    public class VideoStream
    {
        private readonly IntPtr _handle;

        internal VideoStream(Device device, OniSensorType sensorType)
        {
            OpenNI2.oniDeviceCreateStream(device.Handle, sensorType, out _handle);
        }

        public void Close()
        {
            OpenNI2.oniStreamDestroy(_handle);
        }
    }
}
