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
        public event ClientEventHandler OnConnected;
        /// <summary>
        /// Invoked if client lost connection to remote host
        /// </summary>
        public event ClientEventHandler OnDisconnected;
        /// <summary>
        /// Invoked when a packet is sent from local server
        /// </summary>
        public event PacketEventHandler OnPacketSent;
        /// <summary>
        /// Invoked when a packet is received from a remote peer
        /// </summary>
        public event PacketEventHandler OnPacketReceived;
        #endregion

        #region Private fields
        internal PacketProcessor Processor { get; set; }
        private TcpClient _uNetClient;
        private NetworkStream _netStream;
        #endregion

        #region Public fields
        public IPEndPoint EndPoint { get; set; }
        public int BufferSize { get; set; }
        public static OptionSet Settings { get; set; }
        #endregion

        public uNetClient(string host, uint port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            BufferSize = 1024;

            Settings = new OptionSet(false, null, new List<IPacket>());
            Processor = new PacketProcessor(Settings);

            Processor.OnPacketSent += (o, e) =>
            {
                if (OnPacketSent != null)
                    OnPacketSent(o, e);
            };
        }

        public uNetClient(string host, uint port, OptionSet settings)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            BufferSize = 1024;

            Settings = settings;
            Processor = new PacketProcessor(Settings);

            Processor.OnPacketSent += (o, e) =>
            {
                if (OnPacketSent != null)
                    OnPacketSent(o, e);
            };
        }

        public async Task<bool> Connect()
        {
            var flag = await ConnectAsync();

            if (flag)
            {
                if (OnConnected != null)
                    OnConnected(null, new ClientEventArgs());
            }

            return flag;
        }
        public void Disconnect()
        {
            _uNetClient.Client.Disconnect(true);

            if (OnDisconnected != null)
                OnDisconnected(null, new ClientEventArgs());
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
                
                SendPacket(new HandshakePacket(Settings));
            }

            return _uNetClient.Connected;
        }
        private async void ReadAsync()
        {
            var fBuff = new List<byte>();
            var tmpBuff = new byte[BufferSize];

            while (true)
            {
                if (_netStream.CanRead)
                {
                    try
                    {
                        int read = await _netStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);
                        fBuff.AddRange(tmpBuff.Slice(0, read));

                        while (true)
                        {
                            if (fBuff.Count < sizeof(int)) break;

                            int packetSize = BitConverter.ToInt32(fBuff.ToArray(), 0);

                            if (fBuff.Count < packetSize + sizeof (int))
                                break;

                            // Remove length prefix
                            fBuff.RemoveRange(0, sizeof (int));

                            bool verified;
                            var parsedPacket = Processor.ParsePacket(fBuff.Take(packetSize).ToArray(), out verified);
                            fBuff.RemoveRange(0, packetSize);

                            if (!verified)
                            {
                                Disconnect();
                                break;
                            }

                            if (Globals.ReservedPacketIDs.Contains(parsedPacket.ID))
                                InternalOnPacketReceived(parsedPacket);
                            else if (OnPacketReceived != null)
                                OnPacketReceived(null,
                                                 new PacketEventArgs(null, parsedPacket, packetSize + sizeof (int)));

                            if (fBuff.Count < sizeof (int))
                                break;
                        }
                    }
                    catch
                    {
                        Disconnect();
                        break;
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
                case 9999:
                    throw new uNetProtocolException(((ErrorPacket)packet).ErrorMessage);
            }
        }
        #endregion
    }
}
