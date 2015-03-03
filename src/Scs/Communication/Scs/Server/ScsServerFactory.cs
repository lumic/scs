using System.Security.Cryptography.X509Certificates;
using Hik.Communication.Scs.Communication.EndPoints;

namespace Hik.Communication.Scs.Server
{
    /// <summary>
    /// This class is used to create SCS servers.
    /// </summary>
    public static class ScsServerFactory
    {
        /// <summary>
        /// Creates a new SCS Server using an EndPoint.
        /// </summary>
        /// <param name="endPoint">Endpoint that represents address of the server</param>
        /// <returns>Created TCP server</returns>
        public static IScsServer CreateServer(ScsEndPoint endPoint)
        {
            return endPoint.CreateServer();
        }

        /// <summary>
        /// Creates a new SCS SSL Server using an EndPoint and a certificate
        /// </summary>
        /// <param name="endPoint">Endpoint that represents address of the server</param>
        /// <param name="cert">SSL certificate to use</param>
        /// <returns>Created TCP server</returns>
        public static IScsServer CreateSecureServer(ScsEndPoint endPoint, X509Certificate cert)
        {
            return endPoint.CreateSecureServer(cert);
        }
    }
}
