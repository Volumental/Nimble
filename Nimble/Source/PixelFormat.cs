namespace Nimble
{
    public enum PixelFormat
    {
        Depth_1mm = 100,
        Depth_100um = 101,
        Shift_9_2 = 102,
        Shift_9_3 = 103,
        Rgb888 = 200,
        YUV422 = 201,
        Gray8 = 202,
        Gray16 = 203,
        Jpeg = 204
    }

    internal static class PixelFormatExtensions
    {
        public static PixelFormat ToNimblePixelFormat(this Native.OniPixelFormat pixelFormat)
        {
            return (PixelFormat)((int)pixelFormat);
        }

        public static Native.OniPixelFormat ToRaw(this PixelFormat pixelFormat)
        {
            return (Native.OniPixelFormat)((int)pixelFormat);
        }
    }
}
