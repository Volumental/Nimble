using System.Runtime.InteropServices;

namespace Nimble.Native
{
    internal class OpenNI2
    {
        [DllImport("OpenNI2")]
        public static extern void Initialize();
    }
}
