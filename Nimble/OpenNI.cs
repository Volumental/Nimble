using Nimble.Native;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nimble
{
    public class OpenNI
    {
        public void Initialize()
        {
            OpenNI2.oniInitialize(2001); // version 2.1
        }
        public void Shutdown()
        {
            OpenNI2.oniShutdown();
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
    }
}
