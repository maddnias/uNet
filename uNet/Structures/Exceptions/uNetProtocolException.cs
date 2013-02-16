using System;
using System.Runtime.Serialization;

namespace uNet.Structures.Exceptions
{
    [Serializable]
    public class uNetProtocolException : Exception
    {
        public uNetProtocolException() { }

        public uNetProtocolException(string message)
            : base(message) { }

        protected uNetProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}