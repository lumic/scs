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
using System.Security.Cryptography.X509Certificates;

namespace Hik.Communication.Scs.Server.Tcp
{
    /// <summary>
    /// This class is used to create a SSL TCP server.
    /// </summary>
    internal class ScsTcpSslServer : ScsServerBase
    {
        /// <summary>
        /// The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly ScsTcpEndPoint _endPoint;

        private readonly X509Certificate _certificate;

        /// <summary>
        /// Creates a new ScsTcpServer object.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        /// <param name="certificate">The certificate to use</param>
        public ScsTcpSslServer(ScsTcpEndPoint endPoint, X509Certificate certificate)
        {
            _endPoint = endPoint;
            _certificate = certificate;
        }


        /// <summary>
        /// Creates a TCP connection listener.
        /// </summary>
        /// <returns>Created listener object</returns>
        protected override IConnectionListener CreateConnectionListener()
        {
            return new TcpSslConnectionListener(_endPoint, _certificate);
        }
    }
}
