using System.Runtime.InteropServices;

namespace Nimble.Native
{
    internal class OpenNI2
    {
        [DllImport("OpenNI2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status oniInitialize(int version);
    }
}
