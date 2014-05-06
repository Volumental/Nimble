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

    internal enum OniSensorType
    {
	    ONI_SENSOR_IR = 1,
	    ONI_SENSOR_COLOR = 2,
	    ONI_SENSOR_DEPTH = 3,
    }

    internal enum OniPixelFormat
    {
	    // Depth
	    ONI_PIXEL_FORMAT_DEPTH_1_MM = 100,
	    ONI_PIXEL_FORMAT_DEPTH_100_UM = 101,
	    ONI_PIXEL_FORMAT_SHIFT_9_2 = 102,
	    ONI_PIXEL_FORMAT_SHIFT_9_3 = 103,

	    // Color
	    ONI_PIXEL_FORMAT_RGB888 = 200,
	    ONI_PIXEL_FORMAT_YUV422 = 201,
	    ONI_PIXEL_FORMAT_GRAY8 = 202,
	    ONI_PIXEL_FORMAT_GRAY16 = 203,
	    ONI_PIXEL_FORMAT_JPEG = 204,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OniVideoMode
    {
        public OniPixelFormat pixelFormat;
        public int resolutionX;
        public int resolutionY;
        public int fps;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OniFrame
    {
        public int dataSize;
        public IntPtr data; // unmanaged pointer to data

        public OniSensorType sensorType;
        public ulong timestamp;
        public int frameIndex;

        public int width;
        public int height;

        public OniVideoMode videoMode;
        public bool croppingEnabled; // must be 32-bits wide
        public int cropOriginX;
        public int cropOriginY;

        public int stride;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OniSensorInfo
    {
        public OniSensorType sensorType;
        public int numSupportedVideoModes;
        public IntPtr pSupportedVideoModes; // OniVideoMode
    }

    internal delegate void OniNewFrameCallback(IntPtr streamHandle, IntPtr cookie);

    internal class OpenNI2
    {

        // http://www.network-science.de/ascii/  font: "doom"
        //         _     _ _                             __                  _   _                 
        //        | |   (_) |                           / _|                | | (_)                
        //        | |    _| |__  _ __ __ _ _ __ _   _  | |_ _   _ _ __   ___| |_ _  ___  _ __  ___ 
        //        | |   | | '_ \| '__/ _` | '__| | | | |  _| | | | '_ \ / __| __| |/ _ \| '_ \/ __|
        //        | |___| | |_) | | | (_| | |  | |_| | | | | |_| | | | | (__| |_| | (_) | | | \__ \
        //        \_____/_|_.__/|_|  \__,_|_|   \__, | |_|  \__,_|_| |_|\___|\__|_|\___/|_| |_|___/
        //                                       __/ |                                             
        //                                      |___/            
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniInitialize(int version);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniShutdown();

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniGetDeviceList(out IntPtr list, out int length);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniReleaseDeviceList(IntPtr list);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr oniGetExtendedError();


        //        ______           _             __                  _   _                 
        //        |  _  \         (_)           / _|                | | (_)                
        //        | | | |_____   ___  ___ ___  | |_ _   _ _ __   ___| |_ _  ___  _ __  ___ 
        //        | | | / _ \ \ / / |/ __/ _ \ |  _| | | | '_ \ / __| __| |/ _ \| '_ \/ __|
        //        | |/ /  __/\ V /| | (_|  __/ | | | |_| | | | | (__| |_| | (_) | | | \__ \
        //        |___/ \___| \_/ |_|\___\___| |_|  \__,_|_| |_|\___|\__|_|\___/|_| |_|___/
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceOpen(string uri, out IntPtr deviceHandle);
        
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceClose(IntPtr deviceHandle);
        
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceEnableDepthColorSync(IntPtr deviceHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniDeviceDisableDepthColorSync(IntPtr deviceHandle);

        //         _____ _                               __                  _   _                 
        //        /  ___| |                             / _|                | | (_)                
        //        \ `--.| |_ _ __ ___  __ _ _ __ ___   | |_ _   _ _ __   ___| |_ _  ___  _ __  ___ 
        //         `--. \ __| '__/ _ \/ _` | '_ ` _ \  |  _| | | | '_ \ / __| __| |/ _ \| '_ \/ __|
        //        /\__/ / |_| | |  __/ (_| | | | | | | | | | |_| | | | | (__| |_| | (_) | | | \__ \
        //        \____/ \__|_|  \___|\__,_|_| |_| |_| |_|  \__,_|_| |_|\___|\__|_|\___/|_| |_|___/
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniDeviceCreateStream(IntPtr deviceHandle, OniSensorType sensorType, out IntPtr streamHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniStreamDestroy(IntPtr streamHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniStreamStart(IntPtr streamHandle);
        
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniStreamStop(IntPtr streamHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr oniStreamGetSensorInfo(IntPtr streamHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniStreamRegisterNewFrameCallback(IntPtr streamHandle, OniNewFrameCallback handler, IntPtr cookie, out IntPtr callbackHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void oniStreamUnregisterNewFrameCallback(IntPtr streamHandle, IntPtr callbackHandle);

        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniStreamReadFrame(IntPtr streamHandle, out IntPtr framePointer);

        
        /** Set property in the stream. Use the properties listed in OniTypes.h: ONI_STREAM_PROPERTY_..., or specific ones supplied by the device for its streams. */
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniStreamSetProperty(IntPtr streamHandle, int propertyId, IntPtr data, int dataSize);
        /** Get property in the stream. Use the properties listed in OniTypes.h: ONI_STREAM_PROPERTY_..., or specific ones supplied by the device for its streams. */
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniStreamGetProperty(IntPtr streamHandle, int propertyId, IntPtr data, ref int dataSize);

        /** Check if the property is supported the stream. Use the properties listed in OniTypes.h: ONI_STREAM_PROPERTY_..., or specific ones supplied by the device for its streams. */
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool oniStreamIsPropertySupported(IntPtr streamHandle, int propertyId);
    }
}
