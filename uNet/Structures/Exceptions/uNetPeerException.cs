using System;
using System.Runtime.Serialization;

namespace uNet.Structures.Exceptions
{
    [Serializable]
    public class uNetPeerException : Exception
    {
        public uNetPeerException() { }

        public uNetPeerException(string message)
            : base(message) { }

        protected uNetPeerException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
