using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uNet
{
    public class uNetProtocolException : Exception
    {
        public uNetProtocolException() { }

        public uNetProtocolException(string message)
            : base(message) { }
    }

    public class uNetPeerException : Exception
    {
        public uNetPeerException() { }

        public uNetPeerException(string message)
            : base(message) { }
    }
}
