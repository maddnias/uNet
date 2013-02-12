using System;
using System.Collections.Generic;
using System.IO;
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
            Processor = new PacketProcessor();
        }

        public uNetClient(string host, uint port, ICryptoScheme cryptoScheme)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            BufferSize = 1024;
            Processor = new PacketProcessor();

            Globals.CryptoScheme = cryptoScheme;
        }

        public bool Connect()
        {
             var flag = ConnectAsync().Result;

            if (flag)
            {
                SendPacket(new HandshakePacket());

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

                SendPacket(new HandshakePacket());
            }

            return _uNetClient.Connected;
        }
        private async void ReadAsync()
        {
            while (true)
            {
                if (_netStream.CanRead)
                {
                    var fBuff = new List<byte>();
                    var tmpBuff = new byte[BufferSize];
                    var packetSize = -1;

                    try
                    {
                        packetSize = await Processor.CalculatePrefix(_netStream);
                    }
                    catch
                    {
                        Disconnect();
                    }

                    if (packetSize == -1)
                        return;
                    do
                    {
                        int read = 0;
                        try
                        {
                            read = await _netStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);
                        }
                        catch (Exception)
                        {
                            Disconnect();
                        }

                        fBuff.AddRange((read >= BufferSize ? tmpBuff : tmpBuff.Slice(0, read)));
                    } while (_netStream.DataAvailable);

                    var parsedPacket = Processor.ParsePacket(fBuff.ToArray());

                    if (Globals.ReservedPacketIDs.Contains(parsedPacket.ID))
                        InternalOnPacketReceived(parsedPacket);
                }
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
