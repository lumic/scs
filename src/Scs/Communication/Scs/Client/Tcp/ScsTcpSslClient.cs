#region Copyright Notice
// --------------------------------------------------------------------------------------------------------------------
// Copyright (C) 2015 Lumic Ltd
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// WWW: lumic.co.uk
// Email: support@lumic.co.uk
// --------------------------------------------------------------------------------------------------------------------
#endregion

using Hik.Communication.Scs.Communication.Channels;
using Hik.Communication.Scs.Communication.Channels.Tcp;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Hik.Communication.Scs.Client.Tcp
{
    /// <summary>
    /// This class is used to communicate with server over TCP/IP protocol.
    /// </summary>
    internal class ScsTcpSslClient : ScsClientBase
    {
        /// <summary>
        /// The endpoint address of the server.
        /// </summary>
        private readonly ScsTcpEndPoint _serverEndPoint;

        private readonly bool _acceptSelfSignedCerts;

        private readonly string _serverNameOnCertificate;

        /// <summary>
        /// Creates a new ScsTcpClient object.
        /// </summary>
        /// <param name="serverEndPoint">The endpoint address to connect to the server</param>
        /// <param name="serverNameOnCert"></param>
        /// <param name="acceptSelfSignedCerts">whether to allow self signed certificates</param>
        public ScsTcpSslClient(ScsTcpEndPoint serverEndPoint, string serverNameOnCert, bool acceptSelfSignedCerts)
        {
            _serverEndPoint = serverEndPoint;
            _serverNameOnCertificate = serverNameOnCert;
            _acceptSelfSignedCerts = acceptSelfSignedCerts;
        }

        /// <summary>
        /// Creates a communication channel using ServerIpAddress and ServerPort.
        /// </summary>
        /// <returns>Ready communication channel to communicate</returns>
        protected override ICommunicationChannel CreateCommunicationChannel()
        {
            TcpClient client = new TcpClient();
            SslStream sslStream;
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(_serverEndPoint.IpAddress, _serverEndPoint.TcpPort));

                sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
                    );

                sslStream.AuthenticateAsClient(_serverNameOnCertificate);


                return new TcpSslCommunicationChannel(
                    _serverEndPoint, client, sslStream
                    );
            }
            catch (AuthenticationException)
            {
                client.Close();
                throw;
            }

        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                return false;

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && _acceptSelfSignedCerts)
                return true;

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }
    }
}
