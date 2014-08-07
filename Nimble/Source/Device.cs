using Nimble.Native;
using System;

namespace Nimble
{
    public class Device
    {
        private readonly IntPtr _handle;
        private readonly DeviceInfo _deviceInfo;

        internal Device(DeviceInfo deviceInfo)
        {
            var status = OpenNI2.oniDeviceOpen(deviceInfo.Uri, out _handle);
            status.ThrowIfFailed();
            _deviceInfo = deviceInfo;
        }

        public DeviceInfo DeviceInfo { get { return _deviceInfo; } }

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
            var status = OpenNI2.oniDeviceClose(_handle);
            status.ThrowIfFailed();
        }

        public bool SyncColorAndDepth
        {
            set
            {
                if (value)
                {
                    var status = OpenNI2.oniDeviceEnableDepthColorSync(_handle);
                    status.ThrowIfFailed();
                }
                else
                {
                    OpenNI2.oniDeviceDisableDepthColorSync(_handle);
                }
            }
        }

        public IProperties Properties
        {
            get { return null; }
        }
    }
}
