using Nimble.Native;
using System;
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

                var result = new List<DeviceInfo>();

                IntPtr tmp = list;
                for (int i = 0; i < listLength; i++)
                {
                    OniDeviceInfo deviceInfo = (OniDeviceInfo)Marshal.PtrToStructure(tmp, typeof(OniDeviceInfo));
                    result.Add(new DeviceInfo(deviceInfo));
                    tmp += Marshal.SizeOf(deviceInfo);
                }

                OpenNI2.oniReleaseDeviceList(list);
                return result;
            }
        }
    }
}
