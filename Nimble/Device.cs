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

        public void Close()
        {
            OpenNI2.oniDeviceClose(_handle);
        }
    }
}
