using Nimble.Native;
using System;
using System.Runtime.InteropServices;

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

        private void WriteProperty(int propertyId, int s) 
        {
            int n = Marshal.SizeOf(typeof(int));
            IntPtr buffer = Marshal.AllocHGlobal(n);
            Marshal.WriteInt32(buffer, s);
            var status = OpenNI2.oniDeviceSetProperty(_handle, propertyId, buffer, n);
            Marshal.FreeHGlobal(buffer);
            status.ThrowIfFailed();
        }

        private int ReadIntProperty(int propertyId)
        {
            int n = Marshal.SizeOf(typeof(int));
            IntPtr buffer = Marshal.AllocHGlobal(n);
            var status = OpenNI2.oniDeviceGetProperty(_handle, propertyId, buffer, ref n);
            var result = Marshal.ReadInt32(buffer);
            Marshal.FreeHGlobal(buffer);
            status.ThrowIfFailed();
            return result;
        }

        public ImageRegistrationMode ImageRegistration
        {
            get { return (ImageRegistrationMode)ReadIntProperty(5); }
            set { WriteProperty(5, (int)value); }
        }
    }
}
