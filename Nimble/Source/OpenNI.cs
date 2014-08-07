using Nimble.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nimble
{
    public delegate void DeviceInfoCallback(OpenNI source, DeviceInfo deviceInfo);

    public class OpenNI
    {
        private readonly OniDeviceCallbacks _callbacks;
        private IntPtr _callbacksHandle;

        private static bool _initialized;

        public event DeviceInfoCallback DeviceConnected;
        public event DeviceInfoCallback DeviceDisconnected;

        public OpenNI()
        {
            _callbacks = new OniDeviceCallbacks();
            _callbacks.DeviceConnected = OnDeviceConnected;
            _callbacks.DeviceDisconnected = OnDeviceDisconnected;
            _callbacks.DeviceStateChanged = OnDeviceStateChanged;
        }

        public void Initialize()
        {
            if (_initialized) throw new NimbleException("OpenNI already initalized");
            var status = OpenNI2.oniInitialize(2001); // version 2.1
            status.ThrowIfFailed();

            OpenNI2.oniRegisterDeviceCallbacks(_callbacks, IntPtr.Zero, out _callbacksHandle).ThrowIfFailed();
            
            _initialized = true;
        }

        public void Shutdown()
        {
            if (!_initialized) throw new NimbleException("OpenNI not initalized");
            OpenNI2.oniUnregisterDeviceCallbacks(_callbacksHandle);
            OpenNI2.oniShutdown();
            _initialized = false;
        }

        public IEnumerable<DeviceInfo> Devices
        {
            get
            {
                IntPtr list;
                int listLength;
                OpenNI2.oniGetDeviceList(out list, out listLength);

                var devices = Marshaller.EnumerateStructArray<OniDeviceInfo>(list, listLength).
                    Select(x => new DeviceInfo(x)).ToList();

                OpenNI2.oniReleaseDeviceList(list);
                return devices;
            }
        }

        private void OnDeviceConnected(ref OniDeviceInfo deviceInfo, IntPtr cookie)
        {
            Raise(DeviceConnected, this, new DeviceInfo(deviceInfo));
        }

        private void OnDeviceDisconnected(ref OniDeviceInfo deviceInfo, IntPtr cookie)
        {
            Raise(DeviceDisconnected, this, new DeviceInfo(deviceInfo));
        }

        private void OnDeviceStateChanged(ref OniDeviceInfo deviceInfo, IntPtr cookie)
        {
        }

        private void Raise(DeviceInfoCallback e, OpenNI source, DeviceInfo deviceInfo)
        {
            if (e != null) e(source, deviceInfo);
        }
    }
}
