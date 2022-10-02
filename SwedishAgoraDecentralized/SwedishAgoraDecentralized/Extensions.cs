using SwedishAgoraDecentralized.FileDistribution;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static SwedishAgoraDecentralized.Utilities;

namespace SwedishAgoraDecentralized
{
    public static class Extensions
    {
        public static string[] GetParts(this string clientRequest, string prefix) => 
            clientRequest.Contains('*') 
            ? clientRequest[prefix.Length..].Split('*') 
            : new[] { clientRequest[prefix.Length..] } ;

        public static async Task<string?> SendBroadCastMessageAsync(this UdpClient client, Encoding encoding, string requestMessage, bool skipResponse = false)
        {
            IPEndPoint globalEndpoint = new(IPAddress.Broadcast, FileDistributionNodeHelpers.ServerPort);

            var requestData = encoding.GetBytes(requestMessage);
            while (await client.SendAsync(requestData, requestData.Length, globalEndpoint) != requestData.Length)
                Log($"Retrying {requestMessage}");

            if (skipResponse)
                return null;

            var recieveresult = await client.ReceiveAsync();
            return encoding.TryGetString(recieveresult.Buffer);
        }

        public static NetworkStream GetStream(this UdpClient client)
        {
            return new NetworkStream(client.Client);
        }

        public static string? TryGetString(this Encoding encoding, byte[] bytes)
            => bytes == null || bytes.Length == 0 ? null : encoding.GetString(bytes);

        public static void FireAndForget(this Task task) => _ = task;
    }
}