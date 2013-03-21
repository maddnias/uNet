using System;
using System.Collections.Generic;
using System.IO;
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
using uNet.Structures.Settings.Base;
using uNet.Utilities;
using uNet.Utilities.Extensions;

namespace uNet.Client
{
    public class uNetClient : IDisposable
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
        public static OptionSet Settings { get; set; }
        #endregion

        public uNetClient(string host, uint port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            Settings = new ClientSettings(new List<IPacket>
                                              {
                                                  new HandshakePacket(),
                                                  new ErrorPacket()
                                              }, null, false);

            BufferSize = Settings.ReceiveBufferSize;
            Processor = Settings.Processor;
            Processor.Settings = Settings;

            Processor._onPacketSent += (o, e) =>
            {
                if (OnPacketSent != null)
                    OnPacketSent(o, e);
            };
        }

        public uNetClient(string host, uint port, OptionSet settings)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(host), (int)port);
            _uNetClient = new TcpClient();
            _uNetClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
            Settings = settings;

            if (Settings.PacketTable == null)
                Settings.PacketTable = new List<IPacket>
                                           {
                                               new HandshakePacket(),
                                               new ErrorPacket()
                                           };
            else if(settings is ClientSettings)
                Settings.PacketTable.AddRange(new List<IPacket>
                                                  {
                                                      new HandshakePacket(),
                                                      new ErrorPacket()
                                                  });

            BufferSize = Settings.ReceiveBufferSize;
            Processor = Settings.Processor;
            Processor.Settings = Settings;

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

                if (Settings is ClientSettings)
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
            _uNetClient.Close();
            _uNetClient = new TcpClient();
        }
        private async Task<bool> ConnectAsync()
        {
            await _uNetClient.ConnectAsync(EndPoint.Address, EndPoint.Port);

            if (_uNetClient.Connected)
            {
                if (Settings is ClientSettings)
                {
                    if ((Settings as ClientSettings).UseSsl)
                    {
                        _netStream = new SslStream(_uNetClient.GetStream(), false, ValidateServerCertificate, null,
                                                   EncryptionPolicy.RequireEncryption);
                        await
                            (_netStream as SslStream).AuthenticateAsClientAsync(
                                (Settings as ClientSettings).SslServerCertIdentity.CertName);
                    }
                    else
                        _netStream = _uNetClient.GetStream();
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
                catch (ObjectDisposedException e)
                {
                    // Ugly solution but works for now to prevent multiple calls to Disconnect when there
                    // should only be one
                    if (e.ObjectName == "System.Net.Sockets.NetworkStream")
                        break;

                    Disconnect();
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

            return certificateX5092.Thumbprint == (Settings as ClientSettings).SslServerCertIdentity.Thumbprint &&
                   certificateX5092.SubjectName.Name == (Settings as ClientSettings).SslServerCertIdentity.CertName
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

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_uNetClient != null)
                {
                    _netStream.Dispose();
                    _uNetClient.Close();
                    _uNetClient = null;
                }
        }
    }
}
