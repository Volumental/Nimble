
namespace Nimble
{
    public enum Status
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

    public class OpenNI
    {
        public void Initialize()
        {
            Native.OpenNI2.oniInitialize(2001); // version 2.1
        }
        public void Shutdown()
        {
            Native.OpenNI2.oniShutdown();
        }
    }
}
