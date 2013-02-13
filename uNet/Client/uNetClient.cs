using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using uNet.Structures;
using uNet.Structures.Packets;
using uNet.Tools;
using uNet.Tools.Extensions;

namespace uNet.Client
{
    public class uNetClient
    {
        #region Events
        /// <summary>
        /// Invoked if client was successfully connected
        /// </summary>
        public event ClientEventHandler OnClientConnected;
        #endregion

        #region Private fields
        internal PacketProcessor Processor { get; set; }
        private readonly TcpClient _uNetClient;
        private NetworkStream _netStream;
        #endregion

        #region Public fields
        public IPEndPoint EndPoint { get; set; }
        public int BufferSize { get; set; }
        #endregion

        public uNetClient(string host, uint port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            BufferSize = 1024;

            Globals.Settings = new OptionSet(false, null);
            Processor = new PacketProcessor();
        }

        public uNetClient(string host, uint port, OptionSet settings)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            BufferSize = 1024;

            Globals.Settings = settings;
            Processor = new PacketProcessor();
        }


        public bool Connect()
        {
             var flag = ConnectAsync().Result;

            if (flag)
            {
                if (OnClientConnected != null)
                    OnClientConnected(null, new ClientEventArgs());
            }

            return flag;
        }
        public void Disconnect()
        {
            _netStream.Dispose();
            _uNetClient.Close();
        }
        public void SendPacket(IPacket packet)
        {
            Processor.SendPacket(packet, _netStream);
        }

        #region Internal socket operations
        private async Task<bool> ConnectAsync()
        {
            await _uNetClient.ConnectAsync(EndPoint.Address, EndPoint.Port);

            if (_uNetClient.Connected)
            {
                _netStream = _uNetClient.GetStream();
                ReadAsync();

                for(var i = 0;i < 5;i++)
                SendPacket(new HandshakePacket());
            }

            return _uNetClient.Connected;
        }
        private async void ReadAsync()
        {
            var fBuff = new List<byte>();
            var tmpBuff = new byte[BufferSize];
            int read;
            int packetSize;

            while (true)
            {
                if (_netStream.CanRead)
                {
                    try
                    {
                        read = await _netStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);
                        fBuff.AddRange(tmpBuff.Slice(0, read));

                        while (true)
                        {
                            if (fBuff.Count < sizeof(int)) break;

                            packetSize = BitConverter.ToInt32(fBuff.ToArray(), 0);

                            if (fBuff.Count >= packetSize + sizeof(int))
                            {
                                // Remove length prefix
                                fBuff.RemoveRange(0, sizeof(int));

                                var verified = true;
                                var parsedPacket = Processor.ParsePacket(fBuff.Take(packetSize).ToArray(), out verified);
                                fBuff.RemoveRange(0, packetSize);

                                if (!verified)
                                {
                                    Disconnect();
                                    break;
                                }

                                if (Globals.ReservedPacketIDs.Contains(parsedPacket.ID))
                                    InternalOnPacketReceived(parsedPacket);

                                if (fBuff.Count < sizeof(int))
                                    break;
                            }
                            else
                                break;
                        }
                    }
                    catch
                    {
                        Disconnect();
                    }
                }
                else
                    Disconnect();
            }
        }
        /// <summary>
        /// Used for uProtocol specific packets which should not be handled by user
        /// </summary>
        internal void InternalOnPacketReceived(IPacket packet)
        {
            switch (packet.ID)
            {
                case 1:
                    throw new uNetProtocolException(((ErrorPacket)packet).ErrorMessage);
            }
        }
        #endregion
    }
}
