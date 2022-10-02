using SwedishAgoraDecentralized.Shared;
using System.Net;
using System.Net.Sockets;

namespace SwedishAgoraDecentralized.EscrowManagement
{
    public sealed class EscrowServiceProviderNode
    {
        private const int PaymentWeight = 25;
        private async Task CheckFileDistributionNodes()
        {
            List<string> nodeEndpoints = new();
            List<(string, string)> checksums = new();
            foreach(string nodeEndpoint in nodeEndpoints)
            {
                var checksum = await CheckNodeAsync(nodeEndpoint);
                checksums.Add((checksum, nodeEndpoint));
            }

            string mostCommonCheckSum = checksums
                .Select(c => c.Item1)
                .GroupBy(c => c)
                .Select(c => new { key = c.Key, count = c.Count() })
                .OrderByDescending(c => c.count)
                .Select(c => c.key)
                .FirstOrDefault() ?? "";

            var payoutEndpoints = checksums
                .Where(c => c.Item1 == mostCommonCheckSum)
                .Select(c => c.Item2)
                .ToList();

            foreach(var payoutEndpoint in payoutEndpoints)
            {
                await PayoutAsync(payoutEndpoint, PaymentWeight);
            }
        }

        private static Task<string> CheckNodeAsync(string nodeEndpoint) => Task.FromResult(nodeEndpoint);
        private static async Task<bool> PayoutAsync(string nodeEndpoint, decimal payoutAmount) 
        {
            if (IPEndPoint.TryParse(nodeEndpoint, out var address))
            {
                try
                {
                    using NetworkStream stream = await SocketFactory.ConnectNewTcpStreamAsync(address);

                    await stream.WriteAsync(Protocol.Encoding.GetBytes("GPP")); //Get payout details
                    string response = await stream.ReadStringAsync();
                    string[] split = response.Split('*');
                    string recieveAddress = split[0];
                    string recieveCurrency = split[1];
                    //Pay out programatically to recieve address

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //If the nodes endpoint is in an invalid format for some reason, nothing we can do about it, just skip
            return true;
        }
    }
}
