using Nimble.Native;
using System;

namespace Nimble
{
    internal static class StatusExtensions
    {        
        internal static void ThrowIfFailed(this Status status)
        {
            switch (status)
            {        
                case Status.Ok: break;
                case Status.Error: throw new NimbleException("General Error");
                case Status.NotImplemented: throw new NotImplementedException();
                case Status.NotSupported: throw new NimbleException("Not supported");
                case Status.BadParameter: throw new ArgumentException("OpenNI: Bad Parameter");
                case Status.OutOfFlow: throw new NimbleException("OpenNI: OutOfFlow"); // 
                case Status.NoDevice: throw new NimbleException("No such device"); // 
                case Status.TimeOut: throw new TimeoutException();
            }
        }
    }
}
