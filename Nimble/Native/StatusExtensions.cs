using Nimble.Native;
using System;
using System.Runtime.InteropServices;

namespace Nimble.Native
{
    internal static class StatusExtensions
    {        
        internal static void ThrowIfFailed(this Status status)
        {
            if (status == Status.Ok) return;
            //var errorMessage = Marshal.PtrToStringAuto(OpenNI2.oniGetExtendedError());
            var errorMessage = Marshal.PtrToStringAnsi(OpenNI2.oniGetExtendedError());
            switch (status)
            {
                case Status.Error: throw new NimbleException("Error: " + errorMessage);
                case Status.NotImplemented: throw new NotImplementedException();
                case Status.NotSupported: throw new NimbleException("Not supported: " + errorMessage);
                case Status.BadParameter: throw new ArgumentException("Bad Parameter: " + errorMessage);
                case Status.OutOfFlow: throw new NimbleException("OutOfFlow: " + errorMessage);
                case Status.NoDevice: throw new NimbleException("No such device: " + errorMessage);
                case Status.TimeOut: throw new TimeoutException("Time out: " + errorMessage);
            }
            throw new NimbleException("Error: " + errorMessage);
        }
    }
}
