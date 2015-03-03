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

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;

namespace Hik.Communication.Scs.Communication.Channels.Tcp
{
    /// <summary>
    /// This class is used to listen and accept incoming TCP
    /// connection requests on a TCP port.
    /// </summary>
    internal class TcpSslConnectionListener : ConnectionListenerBase
    {
        /// <summary>
        /// The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly ScsTcpEndPoint _endPoint;

        /// <summary>
        /// Server socket to listen incoming connection requests.
        /// </summary>
        private TcpListener _listenerSocket;

        /// <summary>
        /// The thread to listen socket
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        private readonly X509Certificate _certificate;

        /// <summary>
        /// Creates a new TcpConnectionListener for given endpoint.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        /// <param name="certificate"></param>
        public TcpSslConnectionListener(ScsTcpEndPoint endPoint, X509Certificate certificate)
        {
            _endPoint = endPoint;
            _certificate = certificate;
        }

        /// <summary>
        /// Starts listening incoming connections.
        /// </summary>
        public override void Start()
        {
            StartSocket();
            _running = true;
            _thread = new Thread(DoListenAsThread);
            _thread.Start();
        }

        /// <summary>
        /// Stops listening incoming connections.
        /// </summary>
        public override void Stop()
        {
            _running = false;
            StopSocket();
        }

        /// <summary>
        /// Starts listening socket.
        /// </summary>
        private void StartSocket()
        {
            _listenerSocket = new TcpListener(_endPoint.IpAddress ?? IPAddress.Any, _endPoint.TcpPort);
            _listenerSocket.Start();
        }

        /// <summary>
        /// Stops listening socket.
        /// </summary>
        private void StopSocket()
        {
            try
            {
                _listenerSocket.Stop();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Entrance point of the thread.
        /// This method is used by the thread to listen incoming requests.
        /// </summary>
        private void DoListenAsThread()
        {
            while (_running)
            {
                try
                {
                    var client = _listenerSocket.AcceptTcpClient();
                    if (client.Connected)
                    {
                        SslStream sslStream = new SslStream(client.GetStream(), false);
                        sslStream.AuthenticateAsServer(_certificate, false, System.Security.Authentication.SslProtocols.Default, true);

                        OnCommunicationChannelConnected(new TcpSslCommunicationChannel(_endPoint, client, sslStream));
                    }
                }
                catch
                {
                    //Disconnect, wait for a while and connect again.
                    StopSocket();
                    Thread.Sleep(1000);
                    if (!_running)
                    {
                        return;
                    }

                    try
                    {
                        StartSocket();
                    }
                    catch
                    {

                    }
                }
            }
        }


    }
}
