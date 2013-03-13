using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using uNet.Structures.Events;
using uNet.Structures.Exceptions;
using uNet.Structures.Packets;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings;
using uNet.Utilities;
using uNet.Utilities.Extensions;

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
            _uNetClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Settings = new ClientSettings(new List<IPacket>(), null, false);

            BufferSize = Settings.ReceiveBufferSize;
            Processor = new uProtocolProcessor(Settings);

            Processor._onPacketSent += (o, e) =>
            {
                if (OnPacketSent != null)
                    OnPacketSent(o, e);
            };
        }

        public uNetClient(string host, uint port, ClientSettings settings)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            _uNetClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
            Settings = settings;

            BufferSize = Settings.ReceiveBufferSize;
            Processor = new uProtocolProcessor(Settings);

            Processor._onPacketSent += (o, e) =>
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
            ResetSocket();

            if (OnDisconnected != null)
                OnDisconnected(null, new ClientEventArgs());
        }
        public void SendPacket(IPacket packet)
        {
            lock (_sendLock) Processor.SendPacket(packet, _netStream).Wait();
        }

        #region Internal socket operations
        private void ResetSocket()
        {
            _uNetClient.Client.Shutdown(SocketShutdown.Both);
            _uNetClient.Client.Disconnect(false);
            _uNetClient = new TcpClient();
            _uNetClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }
        private async Task<bool> ConnectAsync()
        {
            await _uNetClient.ConnectAsync(EndPoint.Address, EndPoint.Port);

            if (_uNetClient.Connected)
            {
                if (Settings.UseSsl)
                {
                    _netStream = new SslStream(_uNetClient.GetStream(), false, ValidateServerCertificate, null,
                                               EncryptionPolicy.RequireEncryption);
                    await (_netStream as SslStream).AuthenticateAsClientAsync(Settings.SslServerCertIdentity.CertName);
                }
                else
                    _netStream = _uNetClient.GetStream();

                ReadAsync();
            }

            return _uNetClient.Connected;
        }

        private async void ReadAsync()
        {
            var fBuff = new List<byte>();
            var tmpBuff = new byte[BufferSize];

            while (true)
            {
                try
                {
                    var read = await _netStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);

                    if (read == 0)
                    {
                        Disconnect();
                        break;
                    }

                    fBuff.AddRange(tmpBuff.Slice(0, read));

                    Processor.ProcessData(fBuff).ForEach(packet =>
                                                                 {
                                                                     if (
                                                                         Globals.ReservedPacketIDs.Contains(
                                                                             packet.ID))
                                                                         InternalOnPacketReceived(packet);
                                                                     else
                                                                         OnPacketReceived(null,
                                                                                          new PacketEventArgs(null,
                                                                                                              packet));
                                                                 });
                }
                catch
                {
                    Disconnect();
                    break;
                }
            }
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                              SslPolicyErrors sslPolicyErrors)
        {
            var certificateX5092 = certificate as X509Certificate2;
            if (certificateX5092 == null) return false;

            return certificateX5092.Thumbprint == Settings.SslServerCertIdentity.Thumbprint &&
                   certificateX5092.SubjectName.Name == Settings.SslServerCertIdentity.CertName
                   && certificateX5092.NotAfter >= DateTime.Now;
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
