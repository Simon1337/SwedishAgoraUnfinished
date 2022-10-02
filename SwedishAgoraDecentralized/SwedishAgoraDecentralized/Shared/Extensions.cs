using System.Net;
using System.Net.Sockets;
using System.Text;
using static SwedishAgoraDecentralized.Shared.Utilities;
using static SwedishAgoraDecentralized.Shared.Protocol;

namespace SwedishAgoraDecentralized.Shared
{
    public static class Extensions
    {
        public static string[] GetParts(this string clientRequest, string prefix) =>
            clientRequest.Contains('*')
            ? clientRequest[prefix.Length..].Split('*')
            : new[] { clientRequest[prefix.Length..] };

        public static async Task<string?> SendBroadCastMessageAsync(this UdpClient client, Encoding encoding, string requestMessage, bool skipResponse = false)
        {
            IPEndPoint globalEndpoint = new(IPAddress.Broadcast, ServerPort);

            var requestData = encoding.GetBytes(requestMessage);
            while (await client.SendAsync(requestData, requestData.Length, globalEndpoint) != requestData.Length)
                Log($"Retrying {requestMessage}");

            if (skipResponse)
                return null;

            //TODO: Needs to be rewritten to use peer discovery
            var recieveresult = await client.ReceiveAsync();
            return encoding.TryGetString(recieveresult.Buffer);
        }
        public static async Task<string> ReadStringAsync(this NetworkStream stream)
        {
            List<byte[]> packets = new();

            while (stream.DataAvailable)
            {
                byte[] packetMemory = new byte[PacketSize];
                int bytes = await stream.ReadAsync(packetMemory);
                packets.Add(packetMemory);
                if (bytes < PacketSize)
                    break;
            }

            return Protocol.Encoding.TryGetString(packets.SelectMany(p => p).ToArray()) ?? "";
        }

        public static async Task<string> ReadStringAsync(this NetworkStream stream)
        {
            byte[] buffer = new byte[MaxMessageSize];
            await stream.ReadAsync(buffer);
            return Protocol.Encoding.GetString(buffer);
        }

        public static NetworkStream GetStream(this UdpClient client)
            => new NetworkStream(client.Client);

        public static string? TryGetString(this Encoding encoding, byte[] bytes)
            => bytes == null || bytes.Length == 0 ? null : encoding.GetString(bytes);

        public static void FireAndForget(this Task task) => _ = task;
    }
}