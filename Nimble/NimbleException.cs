using Nimble.Native;
using System;
using System.Runtime.Serialization;

namespace Nimble
{
    class NimbleException: Exception, ISerializable
    {
        public NimbleException() { }
        public NimbleException(string message): base(message) { }
        public NimbleException(string message, Exception inner): base(message, inner) { }
        protected NimbleException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
