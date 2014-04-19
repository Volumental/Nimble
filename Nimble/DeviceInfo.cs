using System;

namespace Nimble
{
    public class DeviceInfo
    {
        private readonly string _name;
        private readonly string _vendor;
        private readonly string _uri;
        private readonly int _usbProductId;
        private readonly int _usbVendorId;

        internal DeviceInfo(Native.OniDeviceInfo deviceInfo)
        {
            _name = deviceInfo.name;
            _vendor = deviceInfo.vendor;
            _uri = deviceInfo.uri;
            _usbProductId = deviceInfo.usbProductId;
            _usbVendorId = deviceInfo.usbVendorId;
        }

        public string Name { get { return _name; } }
        public string Uri { get { return _uri; } }

        public Device Open()
        {
            return new Device(this);
        }

    }
}
