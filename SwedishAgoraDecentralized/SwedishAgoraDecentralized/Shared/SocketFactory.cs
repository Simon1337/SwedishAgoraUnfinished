using System.Net;
using System.Net.Sockets;

namespace SwedishAgoraDecentralized.Shared
{
    public static class SocketFactory
    {
        public static async Task<TcpClient> ConnectNewTcpClientAsync(IPEndPoint endpoint)
        {
            TcpClient tcpClient = new() { NoDelay = false, ReceiveTimeout = 10_000, SendTimeout = 10_000 };
            await tcpClient.ConnectAsync(endpoint);
            return tcpClient;
        }

        public static async Task<NetworkStream> ConnectNewTcpStreamAsync(IPEndPoint endPoint) 
            => (await ConnectNewTcpClientAsync(endPoint)).GetStream();


    }
}
