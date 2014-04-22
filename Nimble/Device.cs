using Nimble.Native;
using System;

namespace Nimble
{
    public class Device
    {
        private readonly IntPtr _handle;

        internal Device(DeviceInfo deviceInfo)
        {
            OpenNI2.oniDeviceOpen(deviceInfo.Uri, out _handle);
        }

        internal IntPtr Handle { get { return _handle; } }

        public VideoStream OpenColorStream()
        {
            return new VideoStream(this, OniSensorType.ONI_SENSOR_COLOR);
        }

        public VideoStream OpenDepthStream()
        {
            return new VideoStream(this, OniSensorType.ONI_SENSOR_DEPTH);
        }

        public void Close()
        {
            OpenNI2.oniDeviceClose(_handle);
        }
    }
}
