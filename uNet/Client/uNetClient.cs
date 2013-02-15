using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        private Stream _netStream;
        private readonly object _sendLock = new object();
        #endregion

        #region Public fields
        public IPEndPoint EndPoint { get; set; }
        public int BufferSize { get; set; }
        public static ClientSettings Settings { get; set; }
        #endregion

        public uNetClient(string host, uint port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            Settings = new ClientSettings(new List<IPacket>(), false);

            BufferSize = Settings.ReceiveBufferSize;
            Processor = new PacketProcessor(Settings);

            Processor.OnPacketSent += (o, e) =>
            {
                if (OnPacketSent != null)
                    OnPacketSent(o, e);
            };
        }

        public uNetClient(string host, uint port, ClientSettings settings)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            Settings = settings;

            BufferSize = Settings.ReceiveBufferSize;
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

                SendPacket(new HandshakePacket(Settings));
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
            lock (_sendLock) Processor.SendPacket(packet, _netStream).Wait();
        }

        #region Internal socket operations
        private async Task<bool> ConnectAsync()
        {
            await _uNetClient.ConnectAsync(EndPoint.Address, EndPoint.Port);

            if (_uNetClient.Connected)
            {
                if (Settings.UseSSL)
                {
                    _netStream = new SslStream(_uNetClient.GetStream(), false, ValidateServerCertificate, null, EncryptionPolicy.RequireEncryption);
                    await (_netStream as SslStream).AuthenticateAsClientAsync(Settings.SSLServerCertIdentity.CertName);
                }
                else
                    _netStream = _uNetClient.GetStream();

                ReadAsync();
            }

            return _uNetClient.Connected;
        }
        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // 		"8D13532A207D05A60A0329788C9F34CF7D04B01C"
            var certificateX5092 = certificate as X509Certificate2;
            if (certificateX5092 == null) return false;

            if (certificateX5092.Thumbprint == Settings.SSLServerCertIdentity.Thumbprint && certificateX5092.SubjectName.Name == Settings.SSLServerCertIdentity.CertName) return true;

            return false;
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
