using System;
using System.Runtime.InteropServices;

namespace Nimble.Native
{
    internal enum Status
    {
        Ok = 0,
        Error = 1,
        NotImplemented = 2,
        NotSupported = 3,
        BadParameter = 4,
        OutOfFlow = 5,
        NoDevice = 6,
        TimeOut = 102,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OniDeviceInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string uri;

	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	    public string vendor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	    public string name;

        public ushort usbVendorId;
        public ushort usbProductId;
    }

    internal class OpenNI2
    {
        // Library functions
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniInitialize(int version);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniShutdown();

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniGetDeviceList(out IntPtr list, out int length);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniReleaseDeviceList(IntPtr list);

        // Device functions
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceOpen(string uri, out IntPtr deviceHandle);
        
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceClose(IntPtr deviceHandle);
    }
}
